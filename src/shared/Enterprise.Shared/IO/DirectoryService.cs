using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace Enterprise.Shared.IO;

public interface IDirectoryService
{
    string CreateTempDirectory(string? tempRoot = null);
}

public class DirectoryService(ILogger<DirectoryService> logger) : IDirectoryService
{
    public string CreateTempDirectory(string? tempRoot = null)
    {
        logger.LogDebug("Creating temporary directory. CustomRootProvided={CustomRootProvided}", !string.IsNullOrWhiteSpace(tempRoot));

        var tempDir = Path.Combine(tempRoot ?? Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        logger.LogInformation("Temporary directory created successfully {tempDir}", tempDir);
        return tempDir;
    }
}
