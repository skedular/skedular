using Enterprise.Shared.Database.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Testing.Shared.Fixtures;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.HealthCheckExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddDatabaseHealthCheckShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Register_health_check_with_readiness_tag(ServiceCollection services)
    {
        services.AddDatabaseHealthCheck("Server=localhost;Database=test;User Id=sa;Password=Password123!;TrustServerCertificate=True", "sqlserver");

        var registration = services.BuildServiceProvider().GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations.Single();

        registration.Name.ShouldBe("sqlserver");
        registration.Tags.ShouldContain(HealthCheck.Constants.ReadinessTag);
    }
}
