using Enterprise.Shared.Database;
using Enterprise.Shared.Database.SqlServer;
using Enterprise.Shared.UnitTests.Database.TestSupport;
using Enterprise.Shared.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class WithSqlServerDbContextShould
{
    [Theory]
    [AutoFakeItEasyData(
    [
        typeof(ServiceCollectionFixtureCustomizer),
        typeof(FakeHostEnvironmentFixtureCustomizer),
        typeof(SqlServerConfigurationFixtureCustomizer)
    ])]
    public void Register_db_context_and_supporting_services(ServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddLogging();
        services.WithSqlServerDbContext<SqlServerTestDbContext>(configuration, environment, "main", "sqlserver");

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IDbTransactionBuilder>().ShouldNotBeNull();
        provider.GetRequiredService<IDatabaseMigrationService>().ShouldNotBeNull();

        var options = provider.GetRequiredService<DbContextOptions<SqlServerTestDbContext>>();
        options.Extensions.Select(item => item.GetType().Name).ShouldContain(name => name.Contains("SqlServer"));

        var customOptions = provider.GetRequiredService<CustomDbContextOptions<SqlServerTestDbContext>>();
        customOptions.IsPooled.ShouldBeFalse();
        customOptions.IsPostgisEnabled.ShouldBeTrue();

        provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations.Single().Name.ShouldBe("sqlserver");
    }

    [Theory]
    [AutoFakeItEasyData(
    [
        typeof(ServiceCollectionFixtureCustomizer),
        typeof(FakeHostEnvironmentFixtureCustomizer),
        typeof(SqlServerConfigurationFixtureCustomizer)
    ])]
    public void Register_pooled_db_context_factory(ServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddLogging();
        services.WithPooledSqlServerDbContextFactoryWithConnectionString<SqlServerTestDbContext>(
            configuration,
            environment,
            "Server=localhost;Database=test;User Id=sa;Password=Password123!;TrustServerCertificate=True",
            "sqlserver-factory");

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IDbContextFactory<SqlServerTestDbContext>>().ShouldNotBeNull();
        provider.GetRequiredService<CustomDbContextOptions<SqlServerTestDbContext>>().IsPooled.ShouldBeTrue();
    }
}
