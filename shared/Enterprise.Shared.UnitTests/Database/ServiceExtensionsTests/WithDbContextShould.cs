using Enterprise.Shared.Database;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class WithDbContextShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Register_DbContext_Scoped_ServiceLifetime_By_Default(IHostEnvironment hostEnvironment)
    {
        var serviceCollection = new ServiceCollection();
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, null!);

        dbSetupContext.WithPooledDbContextFactory<DummyDbContext>(Migration.None, hostEnvironment);

        var descriptor =
            serviceCollection.First(x => x.ServiceType == typeof(DummyDbContext));

        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Register_DbContext_ServiceLifetime_SingleTon(IHostEnvironment hostEnvironment)
    {
        const ServiceLifetime ServiceLifetime = ServiceLifetime.Singleton;
        var serviceCollection = new ServiceCollection();
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, null!);

        dbSetupContext.WithPooledDbContextFactory<DummyDbContext>(Migration.None, hostEnvironment);

        var descriptor =
            serviceCollection.First(x => x.ServiceType == typeof(DummyDbContext));

        descriptor.Lifetime.Should().Be(ServiceLifetime);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Register_DbContext_ServiceLifetime_Transient(IHostEnvironment hostEnvironment)
    {
        const ServiceLifetime ServiceLifetime = ServiceLifetime.Transient;
        var serviceCollection = new ServiceCollection();
        var dbSetupContext = new DatabaseSetupStub(serviceCollection, null!);

        dbSetupContext.WithPooledDbContextFactory<DummyDbContext>(Migration.None, hostEnvironment);

        var descriptor =
            serviceCollection.First(x => x.ServiceType == typeof(DummyDbContext));

        descriptor.Lifetime.Should().Be(ServiceLifetime);
    }
}
