namespace Enterprise.Shared.Security.Token;

public interface ITokenService
{
    Task VerifyTokenAsync(string token, CancellationToken cancellationToken);
}
