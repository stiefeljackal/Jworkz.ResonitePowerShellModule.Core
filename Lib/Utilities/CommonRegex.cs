using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class CommonRegex
{
    public static readonly Regex RecordPostfixIdRegex = new("^R\\-[0-9A-F]{8}\\-([0-9A-F]{4}\\-){3}[0-9A-F]{12}", RegexOptions.Compiled);

    public static readonly Regex RecordUriRegex = new("^resrec://", RegexOptions.Compiled);
}
