using Enterprise.Shared.Database;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class DatabaseSetupStub : DatabaseSetup
{
    protected internal DatabaseSetupStub(
        IServiceCollection serviceCollection,
        NpgsqlDataSource dataSource)
        : base(serviceCollection, dataSource)
    {
    }
}
