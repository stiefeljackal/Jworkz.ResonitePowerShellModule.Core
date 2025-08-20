using MimeDetective;

namespace Jworkz.ResonitePowerShellModule.Core.Models.Abstract;

public interface IFileSystem
{
    Stream OpenRead(string path);

    Stream CreateFileStream(string path, FileMode fileMode);

    bool DirectoryExists(string path);

    void CreateDirectory(string path);

    FileType? GetFileType(byte[]? bytes);

    FileType? GetFileType(Stream stream);

    void RenameFile(string oldPath, string newPath, bool overwrite);
}
