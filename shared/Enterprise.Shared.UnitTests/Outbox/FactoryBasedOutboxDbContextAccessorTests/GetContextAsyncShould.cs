using Enterprise.Shared.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Outbox.FactoryBasedOutboxDbContextAccessorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetContextAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_new_context_on_get_context_async(
        IDbContextFactory<DbContext> factory,
        ILogger<FactoryBasedOutboxDbContextAccessor<DbContext>> logger,
        CancellationToken cancellationToken)
    {
        var fakeContext1 = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));
        var fakeContext2 = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));

        A.CallTo(() => factory.CreateDbContextAsync(cancellationToken))
            .ReturnsNextFromSequence(Task.FromResult(fakeContext1), Task.FromResult(fakeContext2));

        var sut = new FactoryBasedOutboxDbContextAccessor<DbContext>(factory, logger);

        var context1 = await sut.GetContextAsync(cancellationToken);
        var context2 = await sut.GetContextAsync(cancellationToken);

        context1.ShouldBeSameAs(fakeContext1);
        context2.ShouldBeSameAs(fakeContext2);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Pass_cancellation_token_to_factory(
        IDbContextFactory<DbContext> factory,
        ILogger<FactoryBasedOutboxDbContextAccessor<DbContext>> logger,
        CancellationToken cancellationToken)
    {
        var fakeContext = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));

        A.CallTo(() => factory.CreateDbContextAsync(cancellationToken)).Returns(Task.FromResult(fakeContext));

        var sut = new FactoryBasedOutboxDbContextAccessor<DbContext>(factory, logger);
        await sut.GetContextAsync(cancellationToken);

        A.CallTo(() => factory.CreateDbContextAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
