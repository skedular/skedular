using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
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
    IContext context) : ICustomerHelperService
{
    public async Task<Shared.Database.Entities.Customer> GetCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();

        return customer;
    }

    public async Task<Shared.Database.Entities.Customer> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken) ??
                       throw new CustomerNotFound();

        return customer;
    }

    public async Task<Shared.Models.Customer> UpdateAndPublishEventAsync(
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var customer = mapper.MapTo(await repositoryFactory.CustomerRepository.UpdateAsync(existingCustomer, cancellationToken));
        customerOutboxPublisher.PublishCustomers([customer], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return customer;
    }
}
