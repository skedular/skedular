using AutoFixture;
using Enterprise.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.UnitTests.Fixtures;

public class SqlServerConfigurationFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register<IConfiguration>(() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:main"] = "Server=localhost;Database=test;User Id=sa;Password=Password123!;TrustServerCertificate=True",
                [$"{ApplicationConfiguration.Key}:{nameof(ApplicationConfiguration.QuerySplittingBehavior)}"] =
                    nameof(QuerySplittingBehavior.SplitQuery)
            })
            .Build());
}
