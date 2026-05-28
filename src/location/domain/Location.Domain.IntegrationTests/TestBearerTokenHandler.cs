using System.Net.Http.Headers;

namespace Location.Domain.IntegrationTests;

/// <summary>
///     A delegating handler that injects a <c>Authorization: Bearer</c> header
///     into outgoing HTTP requests when a test has set a verifiable token via
///     <see cref="SetToken" />.  Uses <see cref="AsyncLocal{T}" /> so the token
///     flows correctly through async continuations and is isolated per
///     logical execution context.
/// </summary>
public class TestBearerTokenHandler : DelegatingHandler
{
    private static readonly AsyncLocal<string?> CurrentToken = new();

    public static void SetToken(string token) => CurrentToken.Value = token;

    public static void ClearToken() => CurrentToken.Value = null;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (CurrentToken.Value is not null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentToken.Value);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
