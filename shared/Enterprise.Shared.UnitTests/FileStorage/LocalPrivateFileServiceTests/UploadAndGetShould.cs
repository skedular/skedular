using Enterprise.Shared.Configurations;
using Enterprise.Shared.FileStorage;

namespace Enterprise.Shared.UnitTests.FileStorage.LocalPrivateFileServiceTests;

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
    public async Task Upload_file_and_retrieve_it(string fileName, CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration { ApiBaseDomain = new Uri("https://example.com") };
            var fileStorageConfig = new FileStorageConfiguration { LocalPrivateFilePath = tempDir, PrivateFileEndpoint = "private" };
            var sut = new LocalPrivateFileService(config, fileStorageConfig);
            var content = "private content"u8.ToArray();
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
    public async Task Upload_with_extension_appends_extension(string baseName, CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration { ApiBaseDomain = new Uri("https://example.com") };
            var fileStorageConfig = new FileStorageConfiguration { LocalPrivateFilePath = tempDir, PrivateFileEndpoint = "private" };
            var sut = new LocalPrivateFileService(config, fileStorageConfig);
            using var stream = new MemoryStream("data"u8.ToArray());

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
    public async Task Get_returns_false_for_missing_file(string missingFileName, CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration { ApiBaseDomain = new Uri("https://example.com") };
            var fileStorageConfig = new FileStorageConfiguration { LocalPrivateFilePath = tempDir, PrivateFileEndpoint = "private" };
            var sut = new LocalPrivateFileService(config, fileStorageConfig);

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
    public async Task Get_returns_octet_stream_for_unknown_extension(string baseName, CancellationToken cancellationToken)
    {
        var tempDir = CreateTempDir();
        try
        {
            var config = new ApplicationConfiguration { ApiBaseDomain = new Uri("https://example.com") };
            var fileStorageConfig = new FileStorageConfiguration { LocalPrivateFilePath = tempDir, PrivateFileEndpoint = "private" };
            var sut = new LocalPrivateFileService(config, fileStorageConfig);
            var fileName = baseName + ".unknownext123";
            await File.WriteAllBytesAsync(Path.Combine(tempDir, fileName), "x"u8.ToArray(), cancellationToken);

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
