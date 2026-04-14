using AutoFixture;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Fixtures;

public abstract class InMemoryDbContextFixtureCustomizer<TDbContext> : IFixtureCustomizer where TDbContext : DbContext
{
    private static bool IsPooled => false;

    public void Customize(IFixture fixture) => fixture.Register(CreateDbContext);

    private TDbContext CreateDbContext()
    {
        var databaseName = Guid.CreateVersion7().ToString();
        var genericOptions = new DbContextOptionsBuilder<TDbContext>().UseInMemoryDatabase(databaseName).Options;
        var customOptions = new CustomDbContextOptions<TDbContext> { IsPooled = IsPooled };
        if (TryCreateContext([genericOptions, customOptions], out var created) || TryCreateContext([genericOptions], out created))
        {
            return created;
        }

        var options = new DbContextOptionsBuilder().UseInMemoryDatabase(databaseName).Options;
        if (TryCreateContext([options], out created))
        {
            return created;
        }

        throw new InvalidOperationException(
            $"Could not create {typeof(TDbContext).Name}. Expected a constructor accepting DbContextOptions<TDbContext>, optionally with CustomDbContextOptions<TDbContext>.");
    }

    private static bool TryCreateContext(object[] constructorArguments, out TDbContext created)
    {
        try
        {
            created = (TDbContext)Activator.CreateInstance(typeof(TDbContext), constructorArguments)!;
            return true;
        }
        catch (MissingMethodException)
        {
            created = null!;
            return false;
        }
    }
}
