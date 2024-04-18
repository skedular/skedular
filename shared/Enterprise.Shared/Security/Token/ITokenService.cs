using Enterprise.Shared.Context;

namespace Enterprise.Shared.Security.Token;

public interface ITokenService
{
    Task<PropertyBag?> VerifyTokenAsync(string token, CancellationToken cancellationToken);
}
