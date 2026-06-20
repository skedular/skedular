using AutoFixture;
using Organization.Shared.Services.Pricing;
using Testing.Shared;

namespace Organization.Api.UnitTests.Fixtures;

public class PricingCatalogServiceFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<IPricingCatalogVersionService>(() => new PricingCatalogVersionService());
        fixture.Register(() => TimeProvider.System);
    }
}