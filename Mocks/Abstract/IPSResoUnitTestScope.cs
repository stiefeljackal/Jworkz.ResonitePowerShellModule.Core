using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Core.Mocks.Abstract;

public interface IPSResoUnitTestScope<T> where T : PSResoUnitTestScope
{
    T TestScope { get; }
}
