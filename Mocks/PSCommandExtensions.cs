using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Core.Mocks;

public static class PSCommandExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PSCommand AddEchoCommand(this PSCommand psCommand, object value) =>
        psCommand.AddCommand("echo").AddArgument(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PSCommand AddCommand(this PSCommand psCommand, string cmdletString) =>
        AddCommand(psCommand, cmdletString, null);

    public static PSCommand AddCommand(this PSCommand psCommand, string cmdletString, params CommandParameter[]? parameters)
    {
        Command cmd = new(cmdletString);

        foreach (var parameter in parameters ?? [])
        {
            cmd.Parameters.Add(parameter);
        }

        psCommand.AddCommand(cmd);

        return psCommand;
    }
}
