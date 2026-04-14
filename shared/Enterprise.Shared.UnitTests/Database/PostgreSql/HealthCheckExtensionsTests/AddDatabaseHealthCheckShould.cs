using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.UnitTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.HealthCheckExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddDatabaseHealthCheckShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Register_health_check_with_readiness_tag(ServiceCollection services, string uniqueId)
    {
        var dataSource = $"Host=localhost;Database=test_{uniqueId};Username=test;Password=test".BuildDataSource(false);

        services.AddDatabaseHealthCheck(dataSource, "postgres");

        var registration = services.BuildServiceProvider().GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations.Single();

        registration.Name.ShouldBe("postgres");
        registration.Tags.ShouldContain(HealthCheck.Constants.ReadinessTag);
    }
}
