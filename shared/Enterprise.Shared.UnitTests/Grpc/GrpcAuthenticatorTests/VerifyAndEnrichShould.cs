using Enterprise.Shared.Context;
using Enterprise.Shared.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GrpcConstants = Enterprise.Shared.Grpc.Constants;

namespace Enterprise.Shared.UnitTests.Grpc.GrpcAuthenticatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyAndEnrichShould
{
    private static (GrpcAuthenticator Sut, DefaultHttpContext HttpContext, IContext Context) Build()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = A.Fake<IHttpContextAccessor>();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        var context = A.Fake<IContext>();
        var logger = A.Fake<ILogger<GrpcAuthenticator>>();
        return (new GrpcAuthenticator(accessor, context, logger), httpContext, context);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_api_key_header_is_missing(string apiKey)
    {
        var (sut, _, _) = Build();

        Should.Throw<UnauthorizedAccessException>(() => sut.VerifyAndEnrich(apiKey));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_api_key_does_not_match(string expectedKey, string receivedKey)
    {
        var (sut, httpContext, _) = Build();
        httpContext.Request.Headers[GrpcConstants.ApiKey] = receivedKey;

        Should.Throw<UnauthorizedAccessException>(() => sut.VerifyAndEnrich(expectedKey + "_different"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_throw_when_api_key_matches(string apiKey)
    {
        var (sut, httpContext, _) = Build();
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;

        Should.NotThrow(() => sut.VerifyAndEnrich(apiKey));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_verifiable_token_when_header_present(string apiKey, string token)
    {
        var (sut, httpContext, context) = Build();
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;
        httpContext.Request.Headers[GrpcConstants.VerifiableTokenKey] = token;

        sut.VerifyAndEnrich(apiKey);

        A.CallTo(() => context.SetVerifiableToken(token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_first_verifiable_token_when_multiple_in_header(string apiKey, string token1, string token2)
    {
        var (sut, httpContext, context) = Build();
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;
        httpContext.Request.Headers[GrpcConstants.VerifiableTokenKey] = $"{token1},{token2}";

        sut.VerifyAndEnrich(apiKey);

        A.CallTo(() => context.SetVerifiableToken(token1)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_set_verifiable_token_when_header_is_whitespace(string apiKey)
    {
        var (sut, httpContext, context) = Build();
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;
        httpContext.Request.Headers[GrpcConstants.VerifiableTokenKey] = "   ";

        sut.VerifyAndEnrich(apiKey);

        A.CallTo(() => context.SetVerifiableToken(A<string>._)).MustNotHaveHappened();
    }
}
