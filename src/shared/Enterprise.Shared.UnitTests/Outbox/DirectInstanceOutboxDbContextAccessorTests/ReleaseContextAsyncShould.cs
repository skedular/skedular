using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Logging;
using Testing.Shared.Database.TestSupport;
using Testing.Shared.Fixtures;

namespace Enterprise.Shared.UnitTests.Outbox.DirectInstanceOutboxDbContextAccessorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseContextAsyncShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(DatabaseTestContextFixtureCustomizer)])]
    public async Task Clear_change_tracker_on_release_context_async(
        DatabaseTestContext context,
        ILogger<GetContextAccessor<DatabaseTestContext>> logger,
        CancellationToken cancellationToken)
    {
        await using var _ = context;
        var sut = new GetContextAccessor<DatabaseTestContext>(context, logger);

        await context.Parents.AddAsync(new ParentEntity { Id = Guid.CreateVersion7().ToString() }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        context.Parents.First().Name = "updated";

        context.ChangeTracker.Entries().Any().ShouldBeTrue();

        var accessedContext = await sut.GetContextAsync(cancellationToken);
        await sut.ReleaseContextAsync(accessedContext, cancellationToken);

        accessedContext.ChangeTracker.Entries().Any().ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(DatabaseTestContextFixtureCustomizer)])]
    public async Task Not_dispose_context_on_release(
        DatabaseTestContext context,
        ILogger<GetContextAccessor<DatabaseTestContext>> logger,
        CancellationToken cancellationToken)
    {
        await using var _ = context;
        var sut = new GetContextAccessor<DatabaseTestContext>(context, logger);

        var accessedContext = await sut.GetContextAsync(cancellationToken);
        await sut.ReleaseContextAsync(accessedContext, cancellationToken);

        var act = () => accessedContext.Parents.Count();

        act.ShouldNotThrow();
    }
}
