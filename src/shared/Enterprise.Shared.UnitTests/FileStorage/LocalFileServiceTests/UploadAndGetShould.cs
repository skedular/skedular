using Enterprise.Shared.Configurations;
using Enterprise.Shared.FileStorage;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.FileStorage.LocalFileServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UploadAndGetShould
{
    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Upload_file_and_retrieve_it(
        string fileName,
        ILogger<LocalFileService> logger,
        CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration
            {
                ApiBaseDomain = new Uri("https://example.com"),
            };
            var fileStorageConfig = new FileStorageConfiguration
            {
                FileServerFilePath = tempDir,
                FileEndpoint = "files",
            };
            var sut = new LocalFileService(config, fileStorageConfig, logger);
            var content = "image content"u8.ToArray();
            using var stream = new MemoryStream(content);

            var uri = await sut.UploadAsync(stream, "text/plain", fileName, null, cancellationToken);

            uri.ToString().ShouldContain(fileName);

            var (exists, contentType, bytes) = await sut.GetAsync(fileName, cancellationToken);
            exists.ShouldBeTrue();
            bytes.ShouldBe(content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Upload_with_extension_appends_extension(
        string baseName,
        ILogger<LocalFileService> logger,
        CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration
            {
                ApiBaseDomain = new Uri("https://example.com"),
            };
            var fileStorageConfig = new FileStorageConfiguration
            {
                FileServerFilePath = tempDir,
                FileEndpoint = "files",
            };
            var sut = new LocalFileService(config, fileStorageConfig, logger);
            using var stream = new MemoryStream([.. "data"u8]);

            var uri = await sut.UploadAsync(stream, "text/plain", baseName, ".pdf", cancellationToken);

            uri.ToString().ShouldContain($"{baseName}.pdf");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Get_returns_false_for_missing_file(
        string missingFileName,
        ILogger<LocalFileService> logger,
        CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration
            {
                ApiBaseDomain = new Uri("https://example.com"),
            };
            var fileStorageConfig = new FileStorageConfiguration
            {
                FileServerFilePath = tempDir,
                FileEndpoint = "files",
            };
            var sut = new LocalFileService(config, fileStorageConfig, logger);

            var (exists, contentType, bytes) = await sut.GetAsync(missingFileName, cancellationToken);

            exists.ShouldBeFalse();
            contentType.ShouldBe(string.Empty);
            bytes.ShouldBeEmpty();
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Get_returns_octet_stream_for_unknown_extension(
        string baseName,
        ILogger<LocalFileService> logger,
        CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration
            {
                ApiBaseDomain = new Uri("https://example.com"),
            };
            var fileStorageConfig = new FileStorageConfiguration
            {
                FileServerFilePath = tempDir,
                FileEndpoint = "files",
            };
            var sut = new LocalFileService(config, fileStorageConfig, logger);
            var fileName = baseName + ".unknownext123";
            await File.WriteAllBytesAsync(Path.Combine(tempDir, fileName), [.. "x"u8], cancellationToken);

            var (exists, contentType, _) = await sut.GetAsync(fileName, cancellationToken);

            exists.ShouldBeTrue();
            contentType.ShouldBe("application/octet-stream");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
