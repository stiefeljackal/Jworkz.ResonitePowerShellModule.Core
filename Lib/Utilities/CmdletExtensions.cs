using System.Management.Automation;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

/// <summary>
/// Extension methods for a Cmdlet object
/// </summary>
public static class CmdletExtensions
{
    /// <summary>
    /// Writes an error record stating the password is null or empty
    /// </summary>
    /// <param name="cmdlet">Cmdlet that will write the error record</param>
    public static void WritePasswordEmptyOrNullError(this Cmdlet cmdlet)
    {
        cmdlet.WriteError(new ErrorRecord(new Exception("Provided password is empty or null"), "PWDEMPTYORNULL", ErrorCategory.AuthenticationError, cmdlet));
    }

    /// <summary>
    /// Writes an error record stating the provided credential object is null
    /// </summary>
    /// <param name="cmdlet">Cmdlet that will write the error record</param>
    public static void WriteCredentialNull(this Cmdlet cmdlet)
    {
        cmdlet.WriteError(new ErrorRecord(new Exception("Provided PSCredential object is null"), "CREDENTIALNULL", ErrorCategory.AuthenticationError, cmdlet));
    }
}
