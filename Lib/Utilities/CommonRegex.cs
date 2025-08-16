using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class CommonRegex
{
    public static readonly Regex RecordIdRegex = new(@"^((?<ownerId>[UGM]\-.+)/)?(?<rid>R\-[\w\-_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
}
