using Customer.Api.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerHelperService
{
    Task<Shared.Database.Entities.Customer> GetCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<Shared.Database.Entities.Customer> GetCustomerAsync(CancellationToken cancellationToken);

    Task<Shared.Models.Customer> UpdateAndPublishEventAsync(
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken);
}

public class CustomerHelperService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerOutboxPublisher customerOutboxPublisher,
    IMapper mapper,
    IContext context) : ICustomerHelperService
{
    public async Task<Shared.Database.Entities.Customer> GetCustomerAsync(string customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        var customer =
            await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return customer;
    }

    public async Task<Shared.Database.Entities.Customer> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        var customer =
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                context.PropertyBag.VerifiableToken,
                cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return customer;
    }

    public async Task<Shared.Models.Customer> UpdateAndPublishEventAsync(
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.CustomerRepository.UnitOfWork,
                cancellationToken);
        var customer = mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer));
        await customerOutboxPublisher.PublishCustomerAsync(
            [customer],
            repositoryFactory.CustomerRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return customer;
    }
}
