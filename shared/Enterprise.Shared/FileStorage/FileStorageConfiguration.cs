namespace Enterprise.Shared.FileStorage;

public class FileStorageConfiguration
{
    public const string Key = "FileStorage";
    public bool UseLocal { get; set; } = true;
    public string LocalCdnPath { get; set; } = string.Empty;
    public string PublicCdnFileEndpoint { get; set; } = string.Empty;
    public string LocalPrivateFilePath { get; set; } = string.Empty;
    public string PrivateFileEndpoint { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
}
