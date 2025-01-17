using Enterprise.Shared.Database;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class AddSqlHealthCheckShould
{
    [Fact]
    public void Add_HealthCheck()
    {
        var serviceCollection = new ServiceCollection();
        var dataSource = NpgsqlDataSource.Create("host=123");
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, dataSource);

        dbSetupContext.AddDatabaseHealthCheck();

        serviceCollection.Count.Should().BeGreaterThanOrEqualTo(8); // adds about 8+ things
    }
}
