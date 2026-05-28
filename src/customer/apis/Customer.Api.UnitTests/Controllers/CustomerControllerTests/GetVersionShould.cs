using Customer.Api.Controllers;
using Enterprise.Shared.Version;

namespace Customer.Api.UnitTests.Controllers.CustomerControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetVersionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_GetVersion(
        [Frozen] IVersionService versionService,
        [NoAutoProperties] CustomerCoreController sut,
        CancellationToken cancellationToken)
    {
        _ = await sut.GetVersion(cancellationToken);

        A.CallTo(() => versionService.GetVersion()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(
        [Frozen] IVersionService versionService,
        [NoAutoProperties] CustomerCoreController sut,
        Version version,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => versionService.GetVersion()).Returns(version);

        var result = await sut.GetVersion(cancellationToken);

        result.Value.ShouldNotBeNull();
        result.Value.Major.ShouldBe(version.Major);
        result.Value.Minor.ShouldBe(version.Minor);
        result.Value.Build.ShouldBe(version.Build);
        result.Value.Revision.ShouldBe(version.Revision);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Exception_When_Version_Service_Throws(
        [Frozen] IVersionService versionService,
        [NoAutoProperties] CustomerCoreController sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => versionService.GetVersion()).Throws<Exception>();

        var action = async () => await sut.GetVersion(cancellationToken);

        await action.ShouldThrowAsync<Exception>();
    }
}
