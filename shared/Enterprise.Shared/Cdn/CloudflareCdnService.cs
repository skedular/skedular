using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Enterprise.Shared.Cdn;

public class CloudflareCdnService(CloudflareConfiguration cloudflareConfiguration) : ICdnService
{
    public async Task<(Uri, Uri)> UploadAsync(
        Stream stream,
        string contentType,
        string fileName,
        string? extension,
        CancellationToken cancellationToken)
    {
        var uri = new Uri($"https://{cloudflareConfiguration.AccountId}.r2.cloudflarestorage.com");
        using var client = new AmazonS3Client(
            new BasicAWSCredentials(cloudflareConfiguration.AccessKey, cloudflareConfiguration.SecretKey),
            new AmazonS3Config
            {
                ServiceURL = uri.ToString(),
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            });

        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var request = new PutObjectRequest
        {
            BucketName = cloudflareConfiguration.CdnR2BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = true,
            DisablePayloadSigning = true
        };

        _ = await client.PutObjectAsync(request, cancellationToken);

        return (new Uri(uri, $"{cloudflareConfiguration.CdnR2BucketName}/{fileName}"), new Uri(cloudflareConfiguration.CdnBaseUrl, fileName));
    }

    public Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken) => throw new NotImplementedException();
}
