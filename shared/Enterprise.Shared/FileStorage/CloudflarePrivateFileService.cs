using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Enterprise.Shared.Configurations;
using Flurl;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.FileStorage;

public class CloudflarePrivateFileService(
    ApplicationConfiguration applicationConfiguration,
    FileStorageConfiguration fileStorageConfiguration,
    CloudflareConfiguration cloudflareConfiguration,
    ILogger<CloudflarePrivateFileService> logger)
    : IPrivateFileService
{
    public async Task<Uri> UploadAsync(Stream stream, string contentType, string fileName, string? extension, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Uploading private file to Cloudflare R2. FileName={FileName}, ContentType={ContentType}, Extension={Extension}",
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
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            });

        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var request = new PutObjectRequest
        {
            BucketName = cloudflareConfiguration.PrivateFileR2BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false,
            DisablePayloadSigning = true
        };

        _ = await client.PutObjectAsync(request, cancellationToken);

        logger.LogInformation("Private file uploaded to Cloudflare R2. Bucket={BucketName}, FileName={FileName}",
            cloudflareConfiguration.PrivateFileR2BucketName,
            fileName);

        return new Uri(Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), fileStorageConfiguration.PrivateFileEndpoint, fileName));
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Reading private file from Cloudflare R2. FileName={FileName}", fileName);

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
                new GetObjectMetadataRequest { BucketName = cloudflareConfiguration.PrivateFileR2BucketName, Key = fileName }, cancellationToken);

            var request = new GetObjectRequest { BucketName = cloudflareConfiguration.PrivateFileR2BucketName, Key = fileName };

            using var response = await client.GetObjectAsync(request, cancellationToken);
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

            logger.LogDebug("Private file read succeeded from Cloudflare R2. FileName={FileName}, ContentType={ContentType}",
                fileName,
                response.Headers.ContentType);
            return (true, response.Headers.ContentType, memoryStream.ToArray());
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogWarning(ex, "Failed to read private file from Cloudflare R2. FileName={FileName}", fileName);
            return (false, string.Empty, []);
        }
    }
}
