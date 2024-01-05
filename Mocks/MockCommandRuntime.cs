using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace Jworkz.ResonitePowerShellModule.Mocks;

/// <summary>
/// Mock implementation
/// </summary>
/// <typeparam name="T"></typeparam>
[ExcludeFromCodeCoverage]
public class MockCommandRuntime<T> : ICommandRuntime
{
    private List<object?> _output = new();

    private List<ErrorRecord> _errors = new();

    private List<string> _warnings = new();

    public IEnumerable<T> Output => _output.Cast<T>();

    public IEnumerable<ErrorRecord> Errors => _errors.AsEnumerable();

    public IEnumerable<string> Warnings => _warnings.AsEnumerable();

    public PSHost? Host => throw new NotImplementedException();

    public PSTransactionContext? CurrentPSTransaction => throw new NotImplementedException();

    public bool ShouldContinue(string? query, string? caption) => true;

    public bool ShouldContinue(string? query, string? caption, ref bool yesToAll, ref bool noToAll) => true;

    public bool ShouldProcess(string? target) => true;

    public bool ShouldProcess(string? target, string? action) => true;

    public bool ShouldProcess(string? verboseDescription, string? verboseWarning, string? caption) => true;

    public bool ShouldProcess(string? verboseDescription, string? verboseWarning, string? caption, out ShouldProcessReason shouldProcessReason)
    {
        shouldProcessReason = ShouldProcessReason.None;
        return true;
    }

    [DoesNotReturn]
    public void ThrowTerminatingError(ErrorRecord errorRecord) =>
        throw errorRecord.Exception ?? new InvalidOperationException(errorRecord.ToString());

    public bool TransactionAvailable() => false;

    public virtual void WriteCommandDetail(string text) { }

    public virtual void WriteDebug(string text) { }

    public void WriteError(ErrorRecord errorRecord) => _errors.Add(errorRecord);

    public void WriteObject(object? sendToPipeline) => _output.Add(sendToPipeline);

    public void WriteObject(object? sendToPipeline, bool enumerateCollection)
    {
        if (!enumerateCollection)
        {
            WriteObject(sendToPipeline);
            return;
        }

        var enumerator = LanguagePrimitives.GetEnumerator(sendToPipeline);

        if (enumerator == null)
        {
            WriteObject(sendToPipeline);
            return;
        }

        while (enumerator.MoveNext())
        {
            WriteObject(enumerator.Current);
        }
    }

    public virtual void WriteProgress(ProgressRecord progressRecord) { }

    public virtual void WriteProgress(long sourceId, ProgressRecord progressRecord) { }

    public virtual void WriteVerbose(string text) { }

    public void WriteWarning(string text) => _warnings.Add(text);
}
