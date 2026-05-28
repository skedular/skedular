using Enterprise.Shared.Context;
using Enterprise.Shared.Security.Token;

namespace Team.Api.Services;

internal sealed class FakeTokenService(IContext context) : ITokenService
{
    public Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        context.SetVerifiableToken(token);
        return Task.CompletedTask;
    }
}
