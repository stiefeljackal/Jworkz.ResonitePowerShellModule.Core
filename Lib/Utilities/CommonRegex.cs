using System.Text.RegularExpressions;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class CommonRegex
{
    public static readonly Regex RecordIdRegex = new(@"^((?<ownerId>[UGM]\-.+)/)?(?<rid>R\-[\w\-_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
}
