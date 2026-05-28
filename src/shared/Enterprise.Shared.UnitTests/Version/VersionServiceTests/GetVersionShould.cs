using Enterprise.Shared.Version;

namespace Enterprise.Shared.UnitTests.Version.VersionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetVersionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_non_null_version_for_known_assembly(VersionService<GetVersionShould> sut)
    {
        var version = sut.GetVersion();

        version.ShouldNotBeNull();
    }
}
