using Enterprise.Shared.Version;

namespace Enterprise.Shared.UnitTests.Version.VersionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetVersionShould
{
    [Fact]
    public void Return_non_null_version_for_known_assembly()
    {
        var sut = new VersionService<VersionServiceTests.GetVersionShould>();

        var version = sut.GetVersion();

        version.ShouldNotBeNull();
    }
}
