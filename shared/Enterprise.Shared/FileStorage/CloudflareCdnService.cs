using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Flurl;

namespace Enterprise.Shared.FileStorage;

public class CloudflareCdnService(CloudflareConfiguration cloudflareConfiguration) : ICdnService
{
    public async Task<(Uri, Uri)> UploadAsync(
        Stream stream,
        string contentType,
        string fileName,
        string? extension,
        CancellationToken cancellationToken)
    {
        stream.Position = 0;
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
            AutoCloseStream = false,
            DisablePayloadSigning = true
        };

        _ = await client.PutObjectAsync(request, cancellationToken);

        return (new Uri(Url.Combine(uri.ToString(), cloudflareConfiguration.CdnR2BucketName, fileName)),
            new Uri(Url.Combine(cloudflareConfiguration.CdnBaseUrl.ToString(), fileName)));
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        try
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

            _ = await client.GetObjectMetadataAsync(
                new GetObjectMetadataRequest { BucketName = cloudflareConfiguration.CdnR2BucketName, Key = fileName }, cancellationToken);

            var request = new GetObjectRequest { BucketName = cloudflareConfiguration.CdnR2BucketName, Key = fileName };

            using var response = await client.GetObjectAsync(request, cancellationToken);
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

            return (true, response.Headers.ContentType, memoryStream.ToArray());
        }
        catch (AmazonS3Exception)
        {
            return (false, string.Empty, []);
        }
    }
}
