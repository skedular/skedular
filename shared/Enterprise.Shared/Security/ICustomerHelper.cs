namespace Enterprise.Shared.Security;

public interface ICustomerHelper
{
    ValueTask<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken);
}
