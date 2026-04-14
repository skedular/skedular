using Enterprise.Shared.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Outbox.FactoryBasedOutboxDbContextAccessorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseContextAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Dispose_context_on_release_context_async(CancellationToken cancellationToken)
    {
        var fakeContext = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));
        var factory = A.Fake<IDbContextFactory<DbContext>>();

        A.CallTo(() => factory.CreateDbContextAsync(cancellationToken)).Returns(Task.FromResult(fakeContext));

        var sut = new FactoryBasedOutboxDbContextAccessor<DbContext>(factory);
        var context = await sut.GetContextAsync(cancellationToken);

        await sut.ReleaseContextAsync(context, cancellationToken);

        A.CallTo(() => fakeContext.DisposeAsync()).MustHaveHappenedOnceExactly();
    }
}
