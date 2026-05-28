using Enterprise.Shared.Context;
using Enterprise.Shared.Grpc;
using Microsoft.AspNetCore.Http;
using GrpcConstants = Enterprise.Shared.Grpc.Constants;

namespace Enterprise.Shared.UnitTests.Grpc.GrpcAuthenticatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyAndEnrichShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_api_key_header_is_missing([Frozen] IHttpContextAccessor accessor, GrpcAuthenticator sut, string apiKey)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);

        Should.Throw<UnauthorizedAccessException>(() => sut.VerifyAndEnrich(apiKey));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_api_key_does_not_match(
        [Frozen] IHttpContextAccessor accessor,
        GrpcAuthenticator sut,
        string expectedKey,
        string receivedKey)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        httpContext.Request.Headers[GrpcConstants.ApiKey] = receivedKey;

        Should.Throw<UnauthorizedAccessException>(() => sut.VerifyAndEnrich(expectedKey + "_different"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_throw_when_api_key_matches([Frozen] IHttpContextAccessor accessor, GrpcAuthenticator sut, string apiKey)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;

        Should.NotThrow(() => sut.VerifyAndEnrich(apiKey));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_verifiable_token_when_header_present(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] IContext context,
        GrpcAuthenticator sut,
        string apiKey,
        string token)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;
        httpContext.Request.Headers[GrpcConstants.VerifiableTokenKey] = token;

        sut.VerifyAndEnrich(apiKey);

        A.CallTo(() => context.SetVerifiableToken(token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_first_verifiable_token_when_multiple_in_header(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] IContext context,
        GrpcAuthenticator sut,
        string apiKey,
        string token1,
        string token2)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;
        httpContext.Request.Headers[GrpcConstants.VerifiableTokenKey] = $"{token1},{token2}";

        sut.VerifyAndEnrich(apiKey);

        A.CallTo(() => context.SetVerifiableToken(token1)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_set_verifiable_token_when_header_is_whitespace(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] IContext context,
        GrpcAuthenticator sut,
        string apiKey)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        httpContext.Request.Headers[GrpcConstants.ApiKey] = apiKey;
        httpContext.Request.Headers[GrpcConstants.VerifiableTokenKey] = "   ";

        sut.VerifyAndEnrich(apiKey);

        A.CallTo(() => context.SetVerifiableToken(A<string>._)).MustNotHaveHappened();
    }
}
