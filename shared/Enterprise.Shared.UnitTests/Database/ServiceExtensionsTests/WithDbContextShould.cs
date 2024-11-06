using Enterprise.Shared.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class WithDbContextShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Register_DbContext_Scoped_ServiceLifetime_By_Default(
        IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, null!);

        dbSetupContext.WithPooledDbContextFactory<DummyDbContext>(configuration, Migration.None, hostEnvironment);

        var descriptor =
            serviceCollection.First(x => x.ServiceType == typeof(IDbContextFactory<DummyDbContext>));

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Register_DbContext_ServiceLifetime_SingleTon(
        IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, null!);

        dbSetupContext.WithPooledDbContextFactory<DummyDbContext>(configuration, Migration.None, hostEnvironment);

        var descriptor =
            serviceCollection.First(x => x.ServiceType == typeof(IDbContextFactory<DummyDbContext>));

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Register_DbContext_ServiceLifetime_Transient(
        IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, null!);

        dbSetupContext.WithPooledDbContextFactory<DummyDbContext>(configuration, Migration.None, hostEnvironment);

        var descriptor =
            serviceCollection.First(x => x.ServiceType == typeof(IDbContextFactory<DummyDbContext>));

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}
