using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace Testing.Shared.Fixtures;

public class ServiceCollectionFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register(() => new ServiceCollection());
}
