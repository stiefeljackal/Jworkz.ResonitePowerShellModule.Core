using Jworkz.ResonitePowerShellModule.Core.Mocks.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Core.Mocks;

public class PSResoUnitTest : IPSResoUnitTestScope<PSResoUnitTestScope>
{
    private static PSResoUnitTestScope? _testScope;

    public PSResoUnitTestScope TestScope => GetTestScope();

    public static PSResoUnitTestScope GetTestScope() =>
        _testScope ??= new PSResoUnitTestScope();
}
