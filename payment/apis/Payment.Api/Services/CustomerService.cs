using Api.Shared.Services.Models;
using Enterprise.Shared.Context;
using Enterprise.Shared.Exceptions;
using Payment.Api.Mappers;
using Payment.Shared.Models;
using Payment.Shared.Repositories;

namespace Payment.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<ICollection<StripePaymentMethod>> GetPaymentMethodsAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IMapper mapper, IContext context, ICachedCustomerService cachedCustomerService)
    : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return (mapper.MapTo(customer), customer);
    }

    public async Task<ICollection<StripePaymentMethod>> GetPaymentMethodsAsync(CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await cachedCustomerService.GetAsync(cancellationToken);

        return mapper.MapTo(customerEntity.StripePaymentMethods.Where(item => item.Status == StripePaymentMethodStatusConstants.Confirmed)).ToList();
    }
}
