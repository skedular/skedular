using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;

namespace Customer.Api.Services;

public interface ICustomerHelperService
{
    Task<Shared.Database.Entities.Customer> GetCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<Shared.Database.Entities.Customer> GetCustomerAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> UpdateAndPublishEventAsync(Shared.Database.Entities.Customer existingCustomer, CancellationToken cancellationToken);
}

public class CustomerHelperService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerOutboxPublisher customerOutboxPublisher,
    IMapper mapper,
    IContext context,
    ICachedCustomerService cachedCustomerService) : ICustomerHelperService
{
    public async Task<Shared.Database.Entities.Customer> GetCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();

        return customer;
    }

    public async Task<Shared.Database.Entities.Customer> GetCustomerAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ??
                       throw new CustomerNotFound();

        return customer;
    }

    public async Task<Shared.Models.Customer> UpdateAndPublishEventAsync(
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);
        var customer = mapper.MapTo(existingCustomer);
        customerOutboxPublisher.PublishCustomers([customer], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        existingCustomer = (await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(customer.Id, cancellationToken))!;

        await cachedCustomerService.UpdateAsync([existingCustomer], cancellationToken);

        return customer;
    }
}
