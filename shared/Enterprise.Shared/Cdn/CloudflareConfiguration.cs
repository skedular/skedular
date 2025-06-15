namespace Enterprise.Shared.Cdn;

public class CloudflareConfiguration
{
    public const string Key = "Cloudflare";

    public string AccountId { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string CdnR2BucketName { get; set; } = string.Empty;
    public required Uri CdnBaseUrl { get; set; }
}
