namespace Enterprise.Shared.Security;

public interface ICustomerHelper
{
    Task<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken);
}
