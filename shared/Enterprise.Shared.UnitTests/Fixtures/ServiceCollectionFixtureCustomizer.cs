using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.UnitTests.Fixtures;

public class ServiceCollectionFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register(() => new ServiceCollection());
}
