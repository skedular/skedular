using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Enterprise.Shared;

namespace Gateway.Handlers;

public class ApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApiAuthenticationHttpClientHandler> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        string? outboundRequestSnapshot = null;

        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            var httpRequest = httpContext?.Request;
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
            stopwatch.Stop();

            if (outboundRequestSnapshot is null)
            {
                try
                {
                    outboundRequestSnapshot = await BuildRequestSnapshotAsync(httpRequestMessage, CancellationToken.None);
                }
                catch (Exception snapshotEx)
                {
                    outboundRequestSnapshot = $"<snapshot_failed:{snapshotEx.GetType().Name}>";
                }
            }

            var isCancellation = ex is OperationCanceledException;
            var handlerTokenCanceled = cancellationToken.IsCancellationRequested;
            var requestAborted = httpContextAccessor.HttpContext?.RequestAborted.IsCancellationRequested ?? false;

            logger.LogError(
                ex,
                "Failed to run http query {RequestUri}. Outbound request snapshot: {OutboundRequestSnapshot}. IsCancellation: {IsCancellation}. HandlerTokenCanceled: {HandlerTokenCanceled}. RequestAborted: {RequestAborted}. ElapsedMs: {ElapsedMs}. ExceptionType: {ExceptionType}. InnerExceptionType: {InnerExceptionType}. InnerExceptionMessage: {InnerExceptionMessage}",
                httpRequestMessage.RequestUri,
                outboundRequestSnapshot,
                isCancellation,
                handlerTokenCanceled,
                requestAborted,
                stopwatch.ElapsedMilliseconds,
                ex.GetType().FullName,
                ex.InnerException?.GetType().FullName,
                ex.InnerException?.Message);

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
