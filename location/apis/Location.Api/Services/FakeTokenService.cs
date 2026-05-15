using Enterprise.Shared.Context;
using Enterprise.Shared.Security.Token;

namespace Location.Api.Services;

/// <summary>
///     A development-only token service that accepts any bearer token value and
///     uses it directly as the verifiable token, skipping all JWT validation.
///     Registered only when FAKE_DEPENDENCIES=true so it is never active in
///     production or staging environments.
/// </summary>
internal sealed class FakeTokenService(IContext context) : ITokenService
{
    public Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        context.SetVerifiableToken(token);
        return Task.CompletedTask;
    }
}
