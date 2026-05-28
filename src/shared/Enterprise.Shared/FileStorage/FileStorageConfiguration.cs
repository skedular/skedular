namespace Enterprise.Shared.FileStorage;

public class FileStorageConfiguration
{
    public const string Key = "FileStorage";

    // New naming: filesystem/file-server mode supports local disk and network shares.
    public bool UseFileServer { get; set; } = true;
    public string FileServerPublicFilePath { get; set; } = string.Empty;
    public string PublicCdnFileEndpoint { get; set; } = string.Empty;
    public string FileServerFilePath { get; set; } = string.Empty;
    public string FileEndpoint { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
}
