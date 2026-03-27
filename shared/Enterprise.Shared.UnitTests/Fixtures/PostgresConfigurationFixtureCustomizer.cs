using AutoFixture;
using Enterprise.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.UnitTests.Fixtures;

public class PostgresConfigurationFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register<IConfiguration>(() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:main"] = "Host=localhost;Database=test;Username=test;Password=test",
                [$"{ApplicationConfiguration.Key}:{nameof(ApplicationConfiguration.QuerySplittingBehavior)}"] =
                    nameof(QuerySplittingBehavior.SplitQuery)
            })
            .Build());
}
