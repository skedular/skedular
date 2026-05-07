using Enterprise.Shared.Http;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.UnitTests.Http.HttpExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetCancellationTokenShould
{
    [Fact]
    public void Return_none_when_http_context_is_null()
    {
        var result = ((HttpContext?)null).GetCancellationToken();

        result.ShouldBe(CancellationToken.None);
    }

    [Fact]
    public void Return_request_aborted_token_from_http_context()
    {
        using var cts = new CancellationTokenSource();
        var httpContext = new DefaultHttpContext { RequestAborted = cts.Token };

        var result = httpContext.GetCancellationToken();

        result.ShouldBe(cts.Token);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_none_when_accessor_http_context_is_null([Frozen] IHttpContextAccessor accessor)
    {
        A.CallTo(() => accessor.HttpContext).Returns(null);

        var result = accessor.GetCancellationToken();

        result.ShouldBe(CancellationToken.None);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_token_from_accessor_http_context([Frozen] IHttpContextAccessor accessor)
    {
        using var cts = new CancellationTokenSource();
        var httpContext = new DefaultHttpContext { RequestAborted = cts.Token };
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);

        var result = accessor.GetCancellationToken();

        result.ShouldBe(cts.Token);
    }
}
