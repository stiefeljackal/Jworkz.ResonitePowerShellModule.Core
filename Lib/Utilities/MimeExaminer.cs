using System.Collections.Immutable;
using MimeDetective;
using MimeDetective.Engine;
using MimeDetective.Storage;
using MimeDefinitions = MimeDetective.Definitions;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class MimeExaminer
{
    private static ContentInspector _inspector;

    private static MimeTypeToFileExtensionLookup _mimeToFileExtensions;

    static MimeExaminer()
    {
        var defs = new MimeDefinitions.ExhaustiveBuilder()

        {
            UsageType = MimeDefinitions.Licensing.UsageType.CommercialPaid
        
        }
            .Build()
            .TrimMeta()
            .TrimDescription()
            .ToImmutableArray();

        _inspector = new ContentInspectorBuilder()
        {
            Definitions = defs,
            Parallel = true
        }.Build();

        _mimeToFileExtensions = new MimeTypeToFileExtensionLookupBuilder()
        {
            Definitions = defs
        }.Build();
    }

    public static IEnumerable<DefinitionMatch> Inspect(Stream stream) => _inspector.Inspect(stream);

    public static IEnumerable<DefinitionMatch> Inspect(byte[] bytes) => _inspector.Inspect(bytes);
}
