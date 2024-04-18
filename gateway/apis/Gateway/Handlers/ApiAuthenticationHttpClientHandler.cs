using System.Net.Http.Headers;

namespace Gateway.Handlers;

public class ApiAuthenticationHttpClientHandler(
    IHttpContextAccessor httpContextAccessor,
    ILogger<ApiAuthenticationHttpClientHandler> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (httpContextAccessor.HttpContext == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers.Authorization;
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(token.Scheme, token.Parameter);
            }

            var correlationIdHeader = httpContextAccessor.HttpContext.Request.Headers["X-Correlation-Id"];
            if (!string.IsNullOrWhiteSpace(correlationIdHeader))
            {
                request.Headers.Add("X-Correlation-Id", correlationIdHeader.ToString());
            }

            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to run http query {RequestUri}", request.RequestUri);

            throw;
        }
    }
}
