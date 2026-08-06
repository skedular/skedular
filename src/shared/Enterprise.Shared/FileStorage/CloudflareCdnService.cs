using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Flurl;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.FileStorage;

public class CloudflareCdnService(CloudflareConfiguration cloudflareConfiguration, ILogger<CloudflareCdnService> logger) : ICdnService
{
    public async Task<(Uri, Uri)> UploadAsync(
        Stream stream,
        string contentType,
        string fileName,
        string? extension,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Uploading CDN file to Cloudflare R2. FileName={FileName}, ContentType={ContentType}, Extension={Extension}",
            fileName,
            contentType,
            extension);

        stream.Position = 0;
        var uri = new Uri($"https://{cloudflareConfiguration.AccountId}.r2.cloudflarestorage.com");
        using var client = new AmazonS3Client(
            new BasicAWSCredentials(cloudflareConfiguration.AccessKey, cloudflareConfiguration.SecretKey),
            new AmazonS3Config
            {
                ServiceURL = uri.ToString(),
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
            });

        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var request = new PutObjectRequest
        {
            BucketName = cloudflareConfiguration.CdnR2BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false,
            DisablePayloadSigning = true,
        };

        _ = await client.PutObjectAsync(request, cancellationToken);

        logger.LogInformation("CDN file uploaded to Cloudflare R2. Bucket={BucketName}, FileName={FileName}",
            cloudflareConfiguration.CdnR2BucketName,
            fileName);

        return (new Uri(Url.Combine(uri.ToString(), cloudflareConfiguration.CdnR2BucketName, fileName)),
            new Uri(Url.Combine(cloudflareConfiguration.CdnBaseUrl.ToString(), fileName)));
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Reading CDN file from Cloudflare R2. FileName={FileName}", fileName);

            var uri = new Uri($"https://{cloudflareConfiguration.AccountId}.r2.cloudflarestorage.com");
            using var client = new AmazonS3Client(
                new BasicAWSCredentials(cloudflareConfiguration.AccessKey, cloudflareConfiguration.SecretKey),
                new AmazonS3Config
                {
                    ServiceURL = uri.ToString(),
                    ForcePathStyle = true,
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                    ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
                });

            _ = await client.GetObjectMetadataAsync(
                new GetObjectMetadataRequest
                {
                    BucketName = cloudflareConfiguration.CdnR2BucketName,
                    Key = fileName,
                }, cancellationToken);

            var request = new GetObjectRequest
            {
                BucketName = cloudflareConfiguration.CdnR2BucketName,
                Key = fileName,
            };

            using var response = await client.GetObjectAsync(request, cancellationToken);
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

            logger.LogDebug("CDN file read succeeded from Cloudflare R2. FileName={FileName}, ContentType={ContentType}",
                fileName,
                response.Headers.ContentType);
            return (true, response.Headers.ContentType, memoryStream.ToArray());
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogWarning(ex, "Failed to read CDN file from Cloudflare R2. FileName={FileName}", fileName);
            return (false, string.Empty, []);
        }
    }
}
