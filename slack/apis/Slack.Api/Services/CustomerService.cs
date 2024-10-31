using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Enterprise.Shared.Context;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Customer = Slack.Shared.Models.Customer;
using CustomerConfiguration = Slack.Shared.Configurations.CustomerConfiguration;

namespace Slack.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableCustomerAsync(CancellationToken cancellationToken);

    Task<(Customer, Shared.Database.Entities.Customer)>
        GetCustomerAsync(string id, CancellationToken cancellationToken);

    ValueTask<Customer> GetAsync(WorkspaceMember workspaceMember, CancellationToken cancellationToken);
    ValueTask<Customer> GetByIdAsync(string customerId, CancellationToken cancellationToken);
}

public class CustomerService(
    CustomerConfiguration customerConfiguration,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context,
    global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService.CustomerServiceClient customerServiceClient)
    : ICustomerService, IDisposable
{
    private readonly SemaphoreSlim _cachedCustomerByIdLock = new(1, 1);
    private readonly SemaphoreSlim _cachedCustomerLock = new(1, 1);
    private Customer? _cachedCustomer;
    private Customer? _cachedCustomerById;
    private bool _disposed;

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        CancellationToken cancellationToken)
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

        return (mapper.MapTo(customer)!, customer);
    }

    public async Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableCustomerAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.PropertyBag.VerifiableToken))
        {
            return (null, null);
        }

        var customer =
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                context.PropertyBag.VerifiableToken,
                cancellationToken);
        return customer is null ? (null, null) : (mapper.MapTo(customer)!, customer);
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        string id,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return (mapper.MapTo(customer)!, customer);
    }

    public async ValueTask<Customer>
        GetAsync(WorkspaceMember workspaceMember, CancellationToken cancellationToken)
    {
        if (_cachedCustomer is not null)
        {
            return _cachedCustomer;
        }

        try
        {
            await _cachedCustomerLock.WaitAsync(cancellationToken);
            _cachedCustomer = mapper.MapTo(await customerServiceClient.GetAsync(
                new GetInput(),
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

            return _cachedCustomer;
        }
        finally
        {
            _cachedCustomerLock.Release();
        }
    }

    public async ValueTask<Customer> GetByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        if (_cachedCustomerById is not null)
        {
            return _cachedCustomerById;
        }

        try
        {
            await _cachedCustomerByIdLock.WaitAsync(cancellationToken);
            _cachedCustomerById = mapper.MapTo(await customerServiceClient.Admin_GetAsync(
                new Admin_GetInput { CustomerId = customerId },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

            return _cachedCustomerById;
        }
        finally
        {
            _cachedCustomerByIdLock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CustomerService() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cachedCustomerLock.Dispose();
            _cachedCustomerByIdLock.Dispose();
        }

        _disposed = true;
    }
}
