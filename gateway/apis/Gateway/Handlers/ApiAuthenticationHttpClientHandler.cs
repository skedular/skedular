using System.Net.Http.Headers;
using Api.Shared;

namespace Gateway.Handlers;

public class ApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApiAuthenticationHttpClientHandler> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
        try
        {
            var httpRequest = httpContextAccessor.HttpContext?.Request;
            if (httpRequest is null)
            {
                return await base.SendAsync(httpRequestMessage, cancellationToken);
            }

            var authorizationHeader = httpRequest.Headers.Authorization;
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var token))
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(token.Scheme, token.Parameter);
            }

            var correlationIdHeader = httpRequest.Headers["X-Correlation-Id"];
            if (!string.IsNullOrWhiteSpace(correlationIdHeader))
            {
                httpRequestMessage.Headers.Add("X-Correlation-Id", correlationIdHeader.ToString());
            }

            var ssoCookiesHeader = httpRequest.Headers[Constants.OrganizationSsoCookieHeader];
            if (!string.IsNullOrWhiteSpace(ssoCookiesHeader))
            {
                httpRequestMessage.Headers.Add(Constants.OrganizationSsoCookieHeader, ssoCookiesHeader.ToString());
            }

            return await base.SendAsync(httpRequestMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to run http query {RequestUri}", httpRequestMessage.RequestUri);

            throw;
        }
    }
}
