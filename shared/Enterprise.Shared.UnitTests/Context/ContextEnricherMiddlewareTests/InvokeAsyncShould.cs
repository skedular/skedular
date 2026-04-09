using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.UnitTests.Context.ContextEnricherMiddlewareTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InvokeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Set_correlation_id_from_request_header(string correlationId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Correlation-Id"] = correlationId;

        var context = A.Fake<IContext>();
        var nextCalled = false;

        var sut = new ContextEnricherMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await sut.InvokeAsync(httpContext, context);

        A.CallTo(() => context.SetCorrelationId(correlationId)).MustHaveHappenedOnceExactly();
        nextCalled.ShouldBeTrue();
    }
}
