using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;

namespace Booking.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(string id, CancellationToken cancellationToken);
}

public class CustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context) : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();

        return (mapper.MapTo(customer)!, customer);
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(id, true, cancellationToken) ?? throw new CustomerNotFound();

        return (mapper.MapTo(customer)!, customer);
    }
}
