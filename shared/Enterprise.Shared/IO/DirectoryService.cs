using Path = System.IO.Path;

namespace Enterprise.Shared.IO;

public interface IDirectoryService
{
    string CreateTempDirectory(string? tempRoot = null);
}

public class DirectoryService : IDirectoryService
{
    public string CreateTempDirectory(string? tempRoot = null)
    {
        var tempDir = Path.Combine(tempRoot ?? Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        return tempDir;
    }
}
