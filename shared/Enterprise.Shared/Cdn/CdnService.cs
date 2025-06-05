using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Enterprise.Shared.Cdn;

public interface ICdnService
{
    Task<string> UploadAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken);
}

public class CdnService(Cloudflare cloudflare) : ICdnService
{
    public async Task<string> UploadAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken)
    {
        var uri = new Uri($"https://{cloudflare.AccountId}.r2.cloudflarestorage.com");
        using var client = new AmazonS3Client(
            new BasicAWSCredentials(cloudflare.AccessKey, cloudflare.SecretKey),
            new AmazonS3Config
            {
                ServiceURL = uri.ToString(),
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            });

        var request = new PutObjectRequest
        {
            BucketName = cloudflare.CdnR2BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = true,
            DisablePayloadSigning = true
        };

        _ = await client.PutObjectAsync(request, cancellationToken);

        return new Uri(uri, $"{cloudflare.CdnR2BucketName}/{fileName}").ToString();
    }
}
