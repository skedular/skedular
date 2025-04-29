using Payment.Api.Mappers;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;

namespace Payment.Api.Services;

public interface IWorkaroundService
{
    Task RepublishAllOrganizationStripeConnectAccountsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, IPaymentPublisher paymentPublisher) : IWorkaroundService
{
    public async Task RepublishAllOrganizationStripeConnectAccountsAsync(CancellationToken cancellationToken)
    {
        var accounts = await repositoryFactory.StripeConnectAccountRepository.GetAllAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync(accounts.Select(mapper.MapTo), cancellationToken);
    }
}
