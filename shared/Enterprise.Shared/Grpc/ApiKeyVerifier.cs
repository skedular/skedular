using Enterprise.Shared.Context;
using Enterprise.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Grpc;

public interface IGrpcAuthenticator
{
    void VerifyAndEnrich(string apiKey);
}

public class GrpcAuthenticator(IHttpContextAccessor httpContextAccessor, IContext context) : IGrpcAuthenticator
{
    public void VerifyAndEnrich(string apiKey)
    {
        var receivedKey = httpContextAccessor.HttpContext?.Request.Headers[Constants.ApiKey];
        if (receivedKey is null || string.IsNullOrWhiteSpace(receivedKey.Value.FirstOrDefault()) || receivedKey.Value.First() != apiKey)
        {
            throw new Unauthorized();
        }

        var verifiableTokens = httpContextAccessor.HttpContext?.Request.Headers[Constants.VerifiableTokenKey];
        if (verifiableTokens is not null && !string.IsNullOrWhiteSpace(verifiableTokens.Value.FirstOrDefault()))
        {
            var splitVerifiableTokens = verifiableTokens.Value
                .First()!
                .Split(",")
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();
            if (splitVerifiableTokens.Count != 0)
            {
                context.SetVerifiableToken(splitVerifiableTokens.First());
            }
        }
    }
}
