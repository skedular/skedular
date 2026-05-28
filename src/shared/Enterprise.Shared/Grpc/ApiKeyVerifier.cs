using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Grpc;

public interface IGrpcAuthenticator
{
    void VerifyAndEnrich(string apiKey);
}

public class GrpcAuthenticator(IHttpContextAccessor httpContextAccessor, IContext context, ILogger<GrpcAuthenticator> logger) : IGrpcAuthenticator
{
    public void VerifyAndEnrich(string apiKey)
    {
        logger.LogDebug("Verifying gRPC API key header");

        var receivedKey = httpContextAccessor.HttpContext?.Request.Headers[Constants.ApiKey];
        if (receivedKey is null || string.IsNullOrWhiteSpace(receivedKey.Value.FirstOrDefault()) || receivedKey.Value.First() != apiKey)
        {
            logger.LogWarning("gRPC API key verification failed");
            throw new UnauthorizedAccessException();
        }

        logger.LogDebug("gRPC API key verified successfully");

        var verifiableTokens = httpContextAccessor.HttpContext?.Request.Headers[Constants.VerifiableTokenKey];
        if (verifiableTokens is null || string.IsNullOrWhiteSpace(verifiableTokens.Value.FirstOrDefault()))
        {
            return;
        }

        var splitVerifiableTokens = verifiableTokens.Value
            .First()!
            .Split(",")
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();
        if (splitVerifiableTokens.Count == 0)
        {
            return;
        }

        logger.LogDebug("Applying verifiable token from gRPC headers. TokenCount={TokenCount}", splitVerifiableTokens.Count);
        context.SetVerifiableToken(splitVerifiableTokens.First());
    }
}
