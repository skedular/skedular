using Enterprise.Shared.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Hosting.HostApplicationLifetimeWrapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StopApplicationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Delegate_to_host_application_lifetime(
        IHostApplicationLifetime lifetime,
        ILogger<HostApplicationLifetimeWrapper> logger)
    {
        var sut = new HostApplicationLifetimeWrapper(lifetime, logger);

        sut.StopApplication();

        A.CallTo(() => lifetime.StopApplication()).MustHaveHappenedOnceExactly();
    }
}
