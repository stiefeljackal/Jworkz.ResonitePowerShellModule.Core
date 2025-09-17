using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Core.Mocks;

public class PSResoUnitTestScope : IDisposable
{
    protected Runspace? _runspace;

    protected PowerShell? _ps;

    protected virtual bool IsExecutionPolicyUpdateRequired => true;

    public PSResoUnitTestScope() : this(null)
    {

    }

    public PSResoUnitTestScope(params string[]? modules)
    {
        var initSessionState = InitialSessionState.CreateDefault();

        foreach (var module in modules ?? [])
        {
            initSessionState.ImportPSModule(Path.Join(Directory.GetCurrentDirectory(), module));
        }

        _runspace = RunspaceFactory.CreateRunspace(initSessionState);
        _runspace.Open();

        _ps = PowerShell.Create(_runspace);

        if (IsExecutionPolicyUpdateRequired && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var executionPolicyCmd = new Command("Set-ExecutionPolicy");
            executionPolicyCmd.Parameters.Add("ExecutionPolicy", "Unrestricted");
            _ps.Commands.AddCommand(executionPolicyCmd);
            _ps.Invoke();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<PSObject> ExecuteCmdlet(string cmdletString) =>
        ExecuteCmdlet(cmdletString, null);

    public IEnumerable<PSObject> ExecuteCmdlet(string cmdletString, params CommandParameter[]? parameters)
    {
        PSCommand psCommand = new();
        return ExecuteCmdlet(psCommand.AddCommand(cmdletString, parameters));
    }

    public IEnumerable<PSObject> ExecuteCmdlet(PSCommand psCommand)
    {
        _ps!.Commands = psCommand;
        return _ps.Invoke();
    }

    public void Dispose()
    {
        _ps?.Dispose();
        _runspace?.Dispose();
        GC.SuppressFinalize(this);
    }
}
