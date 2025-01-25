using Enterprise.Shared.Security;

namespace Customer.Api.Services;

public class CustomerHelper(ICachedCustomerService cachedCustomerService) : ICustomerHelper
{
    public async Task<bool> DoesCustomerExistAsync(
        string verifiableToken,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(verifiableToken, cancellationToken);
}
