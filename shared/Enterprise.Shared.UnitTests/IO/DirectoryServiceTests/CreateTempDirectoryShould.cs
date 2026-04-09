using Enterprise.Shared.IO;

namespace Enterprise.Shared.UnitTests.IO.DirectoryServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CreateTempDirectoryShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Create_and_return_a_new_directory(DirectoryService sut)
    {
        var path = sut.CreateTempDirectory();

        try
        {
            path.ShouldNotBeNullOrWhiteSpace();
            Directory.Exists(path).ShouldBeTrue();
        }
        finally
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_provided_temp_root(DirectoryService sut)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempRoot);

        try
        {
            var path = sut.CreateTempDirectory(tempRoot);

            path.ShouldStartWith(tempRoot);
            Directory.Exists(path).ShouldBeTrue();
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }
}
