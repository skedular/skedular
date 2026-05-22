using System.Net.Http.Headers;

namespace Team.Domain.IntegrationTests;

public class TestBearerTokenHandler : DelegatingHandler
{
    private static readonly AsyncLocal<string?> CurrentToken = new();

    public static void SetToken(string token) => CurrentToken.Value = token;

    public static void ClearToken() => CurrentToken.Value = null;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (CurrentToken.Value is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CurrentToken.Value);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
