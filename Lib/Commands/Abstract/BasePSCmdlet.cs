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
            PrepareCmdlet();
        }
        catch (PipelineStoppedException) { throw; }
        catch (Exception ex)
        {
            ExamineThrownException(ex);
        }
    }

    protected override sealed void EndProcessing()
    {
        base.EndProcessing();
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

    protected virtual void PrepareCmdlet() { }

    protected virtual void ExecuteCmdlet() { }

    protected override sealed void ProcessRecord()
    {
        if (_hasError) { return; }

        try
        {
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
