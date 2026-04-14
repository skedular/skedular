using Enterprise.Shared.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Outbox.DirectInstanceOutboxDbContextAccessorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetContextAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_same_context_on_multiple_get_context_async_calls(CancellationToken cancellationToken)
    {
        var fakeContext = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));
        var sut = new GetContextAccessor<DbContext>(fakeContext);

        var context1 = await sut.GetContextAsync(cancellationToken);
        var context2 = await sut.GetContextAsync(cancellationToken);

        context1.ShouldBeSameAs(fakeContext);
        context2.ShouldBeSameAs(fakeContext);
        context1.ShouldBeSameAs(context2);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Ignore_cancellation_token(CancellationToken cancellationToken)
    {
        var fakeContext = A.Fake<DbContext>(options => options.WithArgumentsForConstructor(() => new DbContext(new DbContextOptions<DbContext>())));
        var sut = new GetContextAccessor<DbContext>(fakeContext);

        var context = await sut.GetContextAsync(cancellationToken);

        context.ShouldBeSameAs(fakeContext);
    }
}
