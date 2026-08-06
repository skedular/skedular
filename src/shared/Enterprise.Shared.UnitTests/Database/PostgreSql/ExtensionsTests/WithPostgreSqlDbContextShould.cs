using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Testing.Shared.Database.TestSupport;
using Testing.Shared.Fixtures;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class WithPostgreSqlDbContextShould
{
    [Theory]
    [AutoFakeItEasyData(
    [
        typeof(ServiceCollectionFixtureCustomizer),
        typeof(FakeHostEnvironmentFixtureCustomizer),
        typeof(PostgresConfigurationFixtureCustomizer),
    ])]
    public void Register_db_context_and_supporting_services(ServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddLogging();
        services.WithPostgreSqlDbContext<PostgresTestDbContext>(configuration, environment, "main", true, "postgres");

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IDbTransactionBuilder>().ShouldNotBeNull();
        provider.GetRequiredService<IDatabaseMigrationService>().ShouldNotBeNull();

        var options = provider.GetRequiredService<DbContextOptions<PostgresTestDbContext>>();
        options.Extensions.Select(item => item.GetType().Name).ShouldContain(name => name.Contains("Npgsql"));

        var customOptions = provider.GetRequiredService<CustomDbContextOptions<PostgresTestDbContext>>();
        customOptions.IsPooled.ShouldBeFalse();
        customOptions.IsPostgisEnabled.ShouldBeTrue();

        provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations.Single().Name.ShouldBe("postgres");
    }

    [Theory]
    [AutoFakeItEasyData(
    [
        typeof(ServiceCollectionFixtureCustomizer),
        typeof(FakeHostEnvironmentFixtureCustomizer),
        typeof(PostgresConfigurationFixtureCustomizer),
    ])]
    public void Register_pooled_db_context_factory(ServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddLogging();
        services.WithPooledPostgreSqlDbContextFactoryWithConnectionString<PostgresTestDbContext>(
            configuration,
            environment,
            "Host=localhost;Database=local.test;Username=test;Password=test",
            true,
            "postgres-factory");

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IDbContextFactory<PostgresTestDbContext>>().ShouldNotBeNull();
        provider.GetRequiredService<CustomDbContextOptions<PostgresTestDbContext>>().IsPooled.ShouldBeTrue();
    }
}
