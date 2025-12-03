using AutoFixture.Xunit3;
using Customer.Api.Controllers;
using Enterprise.Shared.Version;
using FakeItEasy;
using FluentAssertions;
using Testing.Shared;

namespace Customer.Api.UnitTests.Controllers.CustomerControllerTests;

public class GetVersionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_GetVersion(
        [Frozen] IVersionService versionService,
        [NoAutoProperties] CustomerController sut,
        CancellationToken cancellationToken)
    {
        _ = await sut.GetVersion(cancellationToken);

        A.CallTo(() => versionService.GetVersion()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(
        [Frozen] IVersionService versionService,
        [NoAutoProperties] CustomerController sut,
        Version version,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => versionService.GetVersion()).Returns(version);

        var result = await sut.GetVersion(cancellationToken);

        result.Value.Should().NotBeNull();
        result.Value.Major.Should().Be(version.Major);
        result.Value.Minor.Should().Be(version.Minor);
        result.Value.Build.Should().Be(version.Build);
        result.Value.Revision.Should().Be(version.Revision);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Exception_When_Version_Service_Throws(
        [Frozen] IVersionService versionService,
        [NoAutoProperties] CustomerController sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => versionService.GetVersion()).Throws<Exception>();

        var action = async () => await sut.GetVersion(cancellationToken);

        await action.Should().ThrowAsync<Exception>();
    }
}
