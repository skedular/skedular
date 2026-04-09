using Enterprise.Shared.Context;
using Enterprise.Shared.Security.Token;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.UnitTests.Security.Token.SecurityContextEnricherMiddlewareTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InvokeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_next_when_no_bearer_token(IContext context, ITokenService tokenService)
    {
        var httpContext = new DefaultHttpContext();
        var nextCalled = false;
        var sut = new SecurityContextEnricherMiddleware(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            [tokenService]);

        await sut.InvokeAsync(httpContext, context);

        nextCalled.ShouldBeTrue();
        A.CallTo(() => tokenService.VerifyTokenAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Verify_token_when_bearer_present(IContext context, ITokenService tokenService, string token)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = $"Bearer {token}";
        var nextCalled = false;
        A.CallTo(() => tokenService.VerifyTokenAsync(token, A<CancellationToken>._)).Returns(Task.CompletedTask);

        var sut = new SecurityContextEnricherMiddleware(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            [tokenService]);

        await sut.InvokeAsync(httpContext, context);

        nextCalled.ShouldBeTrue();
        A.CallTo(() => tokenService.VerifyTokenAsync(token, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
