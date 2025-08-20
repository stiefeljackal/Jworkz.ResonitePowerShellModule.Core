using System.Collections.Concurrent;
using System.Management.Automation;
using Elements.Core;

namespace Jworkz.ResonitePowerShellModule.Core.Commands.Abstract;

using Core.Models;
using Core.Models.Abstract;
using Core.Utilities;

/// <summary>
/// Base class for all cmdlets
/// </summary>
public abstract class BasePSCmdlet : PSCmdlet
{
    private const int WRITE_OBJ_QUEUE_WAIT_DELAY = 100;

    private static readonly ConcurrentQueue<(string Type, string Message)> WRITE_QUEUE = new();

    private bool _hasError = false;

    /// <summary>
    /// Error action specified for this cmdlet if called
    /// </summary>
    public string? ErrorActionSpecified
    {
        get => MyInvocation.BoundParameters["ErrorAction"].ToString()?.ToLowerInvariant();
    }

    public string CurrentLocation
    {
        get => PSState.GetCurrentPwd() ?? string.Empty;
    }

    public IFileSystem FileSystem { get; private set; }

    public IPSState PSState { get; set; }

    /// <summary>
    /// Indicates if the Verbose parameter was specified.
    /// </summary>
    /// <remarks>
    /// This is usually set by the cmdlet; however, this can also be set
    /// programatically.
    /// </remarks>
    public bool IsVerboseSpecified { get; set; } = false;

    public BasePSCmdlet() : this(new FileSystem())
    {
    }

    public BasePSCmdlet(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
        PSState = new PSState(() => SessionState);
    }

    protected override sealed void BeginProcessing()
    {
        IsVerboseSpecified |= IsParamSpecified("Verbose");

        try
        {
            base.BeginProcessing();
            PerformPreprocessSetup();
        }
        catch (PipelineStoppedException) { throw; }
        catch (Exception ex)
        {
            ExamineThrownException(ex);
        }
    }

    protected override sealed void EndProcessing()
    {
        try
        {
            base.EndProcessing();
            CleanUpCmdlet();
        }
        catch (PipelineStoppedException) { throw; }
        catch (Exception ex)
        {
            ExamineThrownException(ex);
        }
    }

    /// <summary>
    /// Checks if a parameter with the provided name has been provided in the execution command
    /// </summary>
    /// <param name="paramName">Name of the parameter to validate if it has been provided in the execution command</param>
    /// <returns>True if a parameter with the provided name is present, false if it is not</returns>
    protected bool IsParamSpecified(string paramName)
    {
        return MyInvocation.BoundParameters.ContainsKey(paramName);
    }

    protected bool HasStoppingErrorAction() =>
        IsParamSpecified("ErrorAction") && (new[] { "stop", "ignore", "silentlycontinue" }).Contains(ErrorActionSpecified);

    protected bool HasIgnoreErrorAction() =>
        IsParamSpecified("ErrorAction") && ErrorActionSpecified == "ignore";

    /// <summary>
    /// Performs any necessary setup during the begin process phase. This is usually
    /// to check if connections are established or other preconditions are met without
    /// any dependency on the parameters passed to the cmdlet.
    /// </summary>
    protected virtual void PerformPreprocessSetup() { }

    /// <summary>
    /// Prepares the cmdlet for execution during the process phase. This is usually
    /// to check if the parameters passed to the cmdlet are valid.
    /// </summary>
    protected virtual void PrepareCmdlet() { }

    /// <summary>
    /// Executes the main logic of the cmdlet during the process phase. This method 
    /// should contain the core functionality the cmdlet and should be overridden in 
    /// derived classes.
    /// </summary>
    protected virtual void ExecuteCmdlet() { }

    /// <summary>
    /// Performs any necessary cleanup during the end process phase.
    /// </summary>
    protected virtual void CleanUpCmdlet() { }

    protected override sealed void ProcessRecord()
    {
        if (_hasError) { return; }

        try
        {
            PrepareCmdlet();
            ExecuteCmdlet();
        }
        catch (PipelineStoppedException) { throw; }
        catch (Exception ex)
        {
            ExamineThrownException(ex);
        }
    }

    public void StartProcessExecution()
    {
        BeginProcessing();
        ProcessRecord();
        EndProcessing();
    }

    protected override void StopProcessing()
    {
        base.StopProcessing();
    }

    /// <summary>
    /// Logs an error message and writes it to the error stream.
    /// </summary>
    /// <remarks>This method creates an <see cref="Exception"/> object using the provided error message and
    /// writes it to the error stream with an error category of <see cref="ErrorCategory.WriteError"/>.</remarks>
    /// <param name="errorMsg">The error message to be logged and written.</param>
    protected void WriteError(string errorMsg)
    {
        ArgumentNullException.ThrowIfNull(errorMsg, nameof(errorMsg));

        Exception ex = new(errorMsg);
        WriteError(ex, ErrorCategory.WriteError);
    }

    /// <summary>
    /// Writes an error record to the error stream based on the provided exception and error category.
    /// </summary>
    /// <remarks>This method creates an <see cref="ErrorRecord"/> using the provided exception and error
    /// category, and writes it to the error stream. The exception's data is augmented with a UTC timestamp to provide
    /// additional context for the error.</remarks>
    /// <param name="ex">The exception that describes the error.</param>
    /// <param name="errorCategory">The category of the error to use to classify the type of issue.</param>
    /// <param name="target">The object associated with the error, or <see langword="null"/> if no specific target is applicable.</param>
    protected void WriteError(Exception ex, ErrorCategory errorCategory, object? target = null)
    {
        ex.Data["TimeStampUtc"] = DateTime.UtcNow;

        ErrorDetails errDetails = new(ex.Message);
        ErrorRecord errRecord = new(ex, "EXCEPTION", ErrorCategory.WriteError, null);
        errRecord.ErrorDetails = errDetails;

        WriteError(errRecord);
    }

    /// <summary>
    /// Examines the provided exception and determines the appropriate error handling action.
    /// </summary>
    /// <remarks>This method sets an internal error state and either throws a new exception or writes the
    /// error based on the current error action configuration. Ensure that the error action is properly configured
    /// before invoking this method.</remarks>
    /// <param name="ex">The exception to be examined.</param>
    /// <exception cref="PSInvalidOperationException">Thrown if the current error action does not indicate a stopping error.</exception>
    private void ExamineThrownException(Exception ex)
    {
        _hasError = true;

        if (!HasStoppingErrorAction())
        {
            throw new PSInvalidOperationException(ex.Message);
        }

        if (!HasIgnoreErrorAction())
        {
            WriteError(ex, ErrorCategory.WriteError);
        }
    }

    /// <summary>
    /// Binds a task to the UniLog system to ensure that log entries are processed while the <see cref="ValueTask{T}"/> is running.
    /// </summary>
    /// <remarks>This method ensures that log entries in the UniLog system are processed while the specified
    /// task is running.  It processes log entries from the write queue and handles them based on their type (e.g.,
    /// info, warning, error). The method blocks until the task is completed and the write queue is empty.  Use this
    /// method when you need to ensure that all log entries are handled during the execution of a task.</remarks>
    /// <typeparam name="T">The type of the result produced by the <see cref="ValueTask{T}"/>.</typeparam>
    /// <param name="task">The <see cref="ValueTask{T}"/> to bind to the UniLog system.</param>
    /// <returns>The original <see cref="ValueTask{T}"/> for further chaining or awaiting.</returns>
    protected ValueTask<T> BindTaskToUniLog<T>(ValueTask<T> task, bool isVerboseEnabled = false)
    {
        BindTaskToUniLog(task.AsTask());
        return task;
    }

    /// <summary>
    /// Binds a task to the UniLog system to ensure that log entries are processed while the task is running.
    /// </summary>
    /// <remarks>This method ensures that log entries in the UniLog system are processed while the specified
    /// task is running.  It processes log entries from the write queue and handles them based on their type (e.g.,
    /// info, warning, error). The method blocks until the task is completed and the write queue is empty.  Use this
    /// method when you need to ensure that all log entries are handled during the execution of a task.</remarks>
    /// <typeparam name="T">The type of the task, which must derive from <see cref="Task"/>.</typeparam>
    /// <param name="task">The task to bind to the UniLog system.</param>
    /// <returns>The original task for further chaining or awaiting.</returns>
    protected T BindTaskToUniLog<T>(T task) where T : Task
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));

        BindToUniLog(IsVerboseSpecified);
        while (!task.IsCompleted || WRITE_QUEUE.Count > 0)
        {
            if (WRITE_QUEUE.TryDequeue(out var entry))
            {
                switch (entry.Type)
                {
                    case "info":
                        WriteVerbose(entry.Message);
                        break;
                    case "warning":
                        WriteWarning(entry.Message);
                        break;
                    case "error":
                        WriteError(entry.Message);
                        break;
                }
            }
            else
            {
                Task.Delay(WRITE_OBJ_QUEUE_WAIT_DELAY).GetAwaiterResult();
            }
        }

        UnbindFromUniLog();

        return task;
    }

    /// <summary>
    /// Binds event handlers to the UniLog logging system to process log messages from it.
    /// </summary>
    /// <remarks>This method attaches handlers to the <c>UniLog.OnWarning</c> and <c>UniLog.OnError</c> events
    /// to process warning and error messages. If <paramref name="includeVerbose"/> is  <see
    /// langword="true"/>, the handler for verbose log messages is also attached to  <c>UniLog.OnLog</c>. Otherwise, the
    /// handler for verbose log messages is detached.</remarks>
    /// <param name="includeVerbose">A value indicating whether verbose log messages should be included.  <see langword="true"/> to include verbose
    /// log messages; otherwise, <see langword="false"/>.</param>
    protected static void BindToUniLog(bool includeVerbose)
    {
        UniLog.OnWarning += EnqueueWarning;
        UniLog.OnError += EnqueueError;

        if (includeVerbose)
        {
            UniLog.OnLog += EnqueueInfo;
        }
        else
        {
            UniLog.OnLog -= EnqueueInfo;
        }
    }

    /// <summary>
    /// Unsubscribes from the UniLog event handlers for info, warning, and error messages.
    /// </summary>
    /// <remarks>This method detaches the current handlers from the <see cref="UniLog.OnWarning"/>,  <see
    /// cref="UniLog.OnError"/>, and <see cref="UniLog.OnLog"/> events to stop processing log messages through
    /// these handlers.</remarks>
    protected static void UnbindFromUniLog()
    {
        UniLog.OnWarning -= EnqueueWarning;
        UniLog.OnError -= EnqueueError;
        UniLog.OnLog -= EnqueueInfo;
    }

    /// <summary>
    /// Adds an informational message to the write queue.
    /// </summary>
    /// <remarks>The message is enqueued with a severity level of "info".</remarks>
    /// <param name="message">The informational message to enqueue.</param>
    private static void EnqueueInfo(string message)
    {
        WRITE_QUEUE.Enqueue(("info", message));
    }

    /// <summary>
    /// Adds a warning message to the write queue.
    /// </summary>
    /// <remarks>The message is enqueued with a severity level of "warning".</remarks>
    /// <param name="message">The warning message to enqueue.</param>
    private static void EnqueueWarning(string message)
    {
        WRITE_QUEUE.Enqueue(("warning", message));
    }

    /// <summary>
    /// Adds a error message to the write queue.
    /// </summary>
    /// <remarks>The message is enqueued with a severity level of "error".</remarks>
    /// <param name="message">The error message to enqueue.</param>
    private static void EnqueueError(string message)
    {
        WRITE_QUEUE.Enqueue(("error", message));
    }
}
