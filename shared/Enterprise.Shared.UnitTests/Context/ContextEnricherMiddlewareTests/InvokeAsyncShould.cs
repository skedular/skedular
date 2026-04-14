using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Context.ContextEnricherMiddlewareTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InvokeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Set_correlation_id_from_request_header(ILogger<ContextEnricherMiddleware> logger, IContext context, string correlationId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Correlation-Id"] = correlationId;

        var nextCalled = false;
        var sut = new ContextEnricherMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, logger);

        await sut.InvokeAsync(httpContext, context);

        A.CallTo(() => context.SetCorrelationId(correlationId)).MustHaveHappenedOnceExactly();
        nextCalled.ShouldBeTrue();
    }
}
