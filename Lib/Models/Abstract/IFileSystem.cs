using MimeDetective;

namespace Jworkz.ResonitePowerShellModule.Core.Models.Abstract;

public interface IFileSystem
{
    Stream OpenRead(string path);

    Stream CreateFileStream(string path, FileMode fileMode);

    FileType? GetFileType(byte[]? bytes);
}
