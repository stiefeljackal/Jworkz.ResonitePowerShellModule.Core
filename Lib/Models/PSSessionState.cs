using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace Jworkz.ResonitePowerShellModule.Core.Models;

using Core.Models.Abstract;

[ExcludeFromCodeCoverage]
internal class PSState : IPSState
{
    private readonly Func<SessionState> _getSessionStateDelegate;

    public SessionState SessionState
    {
        get => _getSessionStateDelegate();
    }

    public PSState(Func<SessionState> getSessionStateDelegate)
    {
        _getSessionStateDelegate = getSessionStateDelegate;
    }

    public string? GetCurrentPwd() => (SessionState.PSVariable.Get("PWD").Value as PathInfo)?.Path;
}
