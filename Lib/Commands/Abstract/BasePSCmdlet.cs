using System.Management.Automation;

namespace Jworkz.ResonitePowerShellModule.Core.Commands.Abstract;

/// <summary>
/// Base class for all cmdlets
/// </summary>
public class BasePSCmdlet : PSCmdlet
{
    private bool _hasError = false;

    /// <summary>
    /// Error action specified for this cmdlet if called
    /// </summary>
    public string? ErrorActionSpecified
    {
        get => MyInvocation.BoundParameters["ErrorAction"].ToString()?.ToLowerInvariant();
    }

    protected override sealed void BeginProcessing()
    {
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

    internal void WriteError(Exception ex, ErrorCategory errorCategory, object? target = null) 
    {
        this.WriteError(new ErrorRecord(ex, string.Empty, errorCategory, target));
    }

    private void ExamineThrownException(Exception ex)
    {
        _hasError = true;
        var errorMessage = ex.Message;

        if (!HasStoppingErrorAction())
        {
            throw new PSInvalidOperationException(errorMessage);
        }

        if (!HasIgnoreErrorAction())
        {
            ex.Data["TimeStampUtc"] = DateTime.UtcNow;

            var errDetails = new ErrorDetails(errorMessage);
            var errRecord = new ErrorRecord(ex, "EXCEPTION", ErrorCategory.WriteError, null);
            errRecord.ErrorDetails = errDetails;

            WriteError(errRecord);
        }
    }
}
