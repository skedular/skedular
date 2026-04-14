using Enterprise.Shared.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.UnitTests.Outbox.ExtentionTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddOutboxDbContextAccessorShould
{
    [Fact]
    public void Register_factory_based_accessor_when_factory_is_registered()
    {
        var services = new ServiceCollection();
        var dbContextFactory = A.Fake<IDbContextFactory<DbContext>>();

        services.AddLogging();
        services.AddSingleton(dbContextFactory);
        services.AddOutboxDbContextAccessor<DbContext>();

        var serviceProvider = services.BuildServiceProvider();
        var accessor = serviceProvider.GetRequiredService<IOutboxDbContextAccessor<DbContext>>();

        accessor.ShouldBeOfType<FactoryBasedOutboxDbContextAccessor<DbContext>>();
    }

    [Fact]
    public void Register_direct_instance_accessor_when_singleton_is_registered()
    {
        var services = new ServiceCollection();
        var dbContext = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));

        services.AddLogging();
        services.AddSingleton(dbContext);
        services.AddOutboxDbContextAccessor<DbContext>();

        var serviceProvider = services.BuildServiceProvider();
        var accessor = serviceProvider.GetRequiredService<IOutboxDbContextAccessor<DbContext>>();

        accessor.ShouldBeOfType<GetContextAccessor<DbContext>>();
    }

    [Fact]
    public void Throw_when_neither_factory_nor_singleton_is_registered()
    {
        var services = new ServiceCollection();

        var act = () => services.AddOutboxDbContextAccessor<DbContext>();

        act.ShouldThrow<InvalidOperationException>();
    }
}
