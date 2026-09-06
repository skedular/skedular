using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.UnitTesting.Fixtures;

public class ServiceCollectionFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register(() => new ServiceCollection());
}
