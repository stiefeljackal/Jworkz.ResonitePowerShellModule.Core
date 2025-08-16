using System.Diagnostics.CodeAnalysis;

using MimeDetective;

namespace Jworkz.ResonitePowerShellModule.Core.Models;

using Abstract;
using Utilities;

[ExcludeFromCodeCoverage]
internal class FileSystem : IFileSystem
{
    public Stream OpenRead(string path) => File.OpenRead(path);

    public Stream CreateFileStream(string path, FileMode fileMode) => new FileStream(path, fileMode);

    public FileType? GetFileType(byte[]? bytes) => MimeExaminer.Inspect(bytes ?? Array.Empty<byte>());
}
