namespace Enterprise.Shared.Cdn;

public class CdnConfiguration
{
    public const string Key = "Cdn";
    public bool UseLocal { get; set; } = true;
    public string LocalCdnPath { get; set; } = string.Empty;
    public Uri LocalCdnBaseUri { get; set; } = Constants.EmptyUri;
    public long MaxFileSize { get; set; }
}
