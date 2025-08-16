using MimeDetective;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class MimeExaminer
{

    public static FileType? Inspect(Stream stream) => stream.GetFileType();

    public static FileType? Inspect(byte[] bytes) => MimeTypes.GetFileType(bytes);

    public static string GetExtension(this FileType? fileType)
    {
        var extension = fileType?.Extension ?? string.Empty;

        return extension.Split(',').First();
    }
}
