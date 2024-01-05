using System.Management.Automation.Host;
using System.Security;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

/// <summary>
/// Extension methods for a PSHost object
/// </summary>
public static class PSHostExtensions
{
    /// <summary>
    /// Requests the host to enter a password that is returned back
    /// </summary>
    /// <param name="host">PSHost to display the prompt</param>
    /// <param name="message">Optional message to display before entering the password</param>
    /// <returns>Password as a SecureString</returns>
    public static SecureString PromptForPassword(this PSHost host, string message = "Enter password: ")
    {
        host.UI.Write(message);
        return host.UI.ReadLineAsSecureString();
    }
}
