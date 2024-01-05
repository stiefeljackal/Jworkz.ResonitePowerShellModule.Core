using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class GeneralUtility
{
    /// <summary>
    /// Checks if Microsoft.PowerShell.SecretManagement module is available
    /// </summary>
    /// <returns></returns>
    public static bool HasPSSecretManagementModule()
    {
        var hasResults = false;
        StartPowerShellSession((ps) =>
        {
            ps.AddCommand("Get-Module")
                .AddParameter("Name", "Microsoft.PowerShell.SecretManagement")
                .AddParameter("ListAvailable");

            var results = ps.Invoke();
            hasResults = results.Any();
        });

        return hasResults;
    }

    /// <summary>
    /// Starts a separate PowerShell session to be used to call commands
    /// </summary>
    /// <param name="callbackFn"></param>
    public static void StartPowerShellSession(Action<PowerShell> callbackFn)
    {
        var initSessionState = InitialSessionState.CreateDefault();

        using (var runspace = RunspaceFactory.CreateRunspace(initSessionState))
        {
            runspace.Open();
            using (var ps = PowerShell.Create(runspace))
            {
                callbackFn(ps);
            }
            runspace.Close();
        }
    }
}
