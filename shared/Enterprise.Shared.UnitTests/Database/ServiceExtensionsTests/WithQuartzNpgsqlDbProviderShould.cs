using Enterprise.Shared.Database;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Quartz.Impl.AdoJobStore.Common;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class WithQuartzNpgsqlDbProviderShould
{
    [Fact]
    public void Register_QuartzNpgsqlDbProvider()
    {
        var serviceCollection = new ServiceCollection();
        var dataSource = NpgsqlDataSource.Create("Host=123");
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, dataSource);

        dbSetupContext.WithQuartzNpgsqlDbProvider();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        serviceProvider
            .GetServices<IDbProvider>()
            .Single()
            .Should()
            .BeAssignableTo<QuartzNpgsqlDbProvider>();
    }
}
