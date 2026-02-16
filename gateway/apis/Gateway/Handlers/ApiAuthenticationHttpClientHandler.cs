using System.Net.Http.Headers;
using System.Text;
using Enterprise.Shared;

namespace Gateway.Handlers;

public class ApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApiAuthenticationHttpClientHandler> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
        string? outboundRequestSnapshot = null;

        try
        {
            var httpRequest = httpContextAccessor.HttpContext?.Request;
            if (httpRequest is not null)
            {
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
            }

            outboundRequestSnapshot = await BuildRequestSnapshotAsync(httpRequestMessage, cancellationToken);
            return await base.SendAsync(httpRequestMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            outboundRequestSnapshot ??= await BuildRequestSnapshotAsync(httpRequestMessage, cancellationToken);

            logger.LogError(
                ex,
                "Failed to run http query {RequestUri}. Outbound request snapshot: {OutboundRequestSnapshot}",
                httpRequestMessage.RequestUri,
                outboundRequestSnapshot);

            throw;
        }
    }

    private static async Task<string> BuildRequestSnapshotAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var builder = new StringBuilder($"{request.Method} {request.RequestUri} HTTP/{request.Version}");
        if (request.Content is null)
        {
            return builder.ToString();
        }

        await request.Content.LoadIntoBufferAsync(cancellationToken);
        var body = await request.Content.ReadAsStringAsync(cancellationToken);
        builder.AppendLine();
        builder.Append(body);

        return builder.ToString();
    }
}
