using Enterprise.Shared.Database;
using Enterprise.Shared.HealthCheck;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.HealthCheckExtensionsTests;

public class HealthCheckExtensionsTests
{
    [Fact]
    public void AddSqlServerHealthCheck_Should_Register_HealthCheck_with_services_tag()
    {
        const string ConnString = "host=123";

        var serviceCollection = new ServiceCollection();

        var dataSource = NpgsqlDataSource.Create(ConnString);

        serviceCollection.AddDatabaseHealthCheck(dataSource);

        // act
        var services = serviceCollection.BuildServiceProvider();

        var registration = services
            .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
            .Value
            .Registrations
            .First();

        registration.Tags.Should().Contain(Constants.ReadinessTag);
    }
}
