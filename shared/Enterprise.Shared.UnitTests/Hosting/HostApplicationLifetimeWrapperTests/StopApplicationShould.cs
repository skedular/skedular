using Enterprise.Shared.Hosting;
using Microsoft.Extensions.Hosting;

namespace Enterprise.Shared.UnitTests.Hosting.HostApplicationLifetimeWrapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StopApplicationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Delegate_to_host_application_lifetime([Frozen] IHostApplicationLifetime lifetime, HostApplicationLifetimeWrapper sut)
    {
        sut.StopApplication();

        A.CallTo(() => lifetime.StopApplication()).MustHaveHappenedOnceExactly();
    }
}
