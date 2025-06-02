namespace Enterprise.Shared.Cdn;

public class Cloudflare
{
    public const string Key = "Cloudflare";

    public string AccountId { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string CdnR2BucketName { get; set; } = string.Empty;
    public string CdnBaseUrl { get; set; } = string.Empty;
}
