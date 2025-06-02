using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Enterprise.Shared.Random;

namespace Enterprise.Shared.Cdn;

public interface ICdnService
{
    Task<string> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken);
}

public class CdnService(Cloudflare cloudflare, IRandomHelper randomHelper) : ICdnService
{
    public async Task<string> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken)
    {
        using var client = new AmazonS3Client(
            new BasicAWSCredentials(cloudflare.AccessKey, cloudflare.SecretKey),
            new AmazonS3Config
            {
                ServiceURL = $"https://{cloudflare.AccountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            });

        var fileName = randomHelper.Generate();
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

        return fileName;
    }
}
