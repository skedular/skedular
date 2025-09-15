using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using Customer = Slack.Shared.Models.Customer;
using CustomerConfiguration = Api.Shared.Clients.Configurations.Grpc.CustomerConfiguration;

namespace Slack.Shared.Services.CrossDomains;

public interface ICustomerService
{
    ValueTask<Customer> GetAsync(WorkspaceMember workspaceMember, CancellationToken cancellationToken);
    Task SubmitFeedbackAsync(WorkspaceMember workspaceMember, string feedback, CancellationToken cancellationToken);
    Task<Customer> AddPreferredLocationAsync(WorkspaceMember workspaceMember, string locationId, CancellationToken cancellationToken);
    Task<Customer> RemovePreferredLocationAsync(WorkspaceMember workspaceMember, string locationId, CancellationToken cancellationToken);
    Task<Customer> AddPreferredTeamAsync(WorkspaceMember workspaceMember, string teamId, CancellationToken cancellationToken);
    Task<Customer> RemovePreferredTeamAsync(WorkspaceMember workspaceMember, string teamId, CancellationToken cancellationToken);
    Task<Customer> AddPreferredOrganizationTagAsync(WorkspaceMember workspaceMember, string organizationTagId, CancellationToken cancellationToken);

    Task<Customer> RemovePreferredOrganizationTagAsync(
        WorkspaceMember workspaceMember,
        string organizationTagId,
        CancellationToken cancellationToken);

    Task<Customer> AddPreferredResourceAsync(WorkspaceMember workspaceMember, string resourceId, CancellationToken cancellationToken);
    Task<Customer> RemovePreferredResourceAsync(WorkspaceMember workspaceMember, string resourceId, CancellationToken cancellationToken);
}

public class CustomerService(
    CustomerConfiguration customerConfiguration,
    Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService.CustomerServiceClient customerServiceClient,
    IMapper mapper,
    IRandomHelper randomHelper)
    : ICustomerService, IDisposable
{
    private readonly SemaphoreSlim _cachedCustomerLock = new(1, 1);
    private Customer? _cachedCustomer;
    private bool _disposed;

    public async ValueTask<Customer> GetAsync(WorkspaceMember workspaceMember, CancellationToken cancellationToken)
    {
        if (_cachedCustomer is not null)
        {
            return _cachedCustomer;
        }

        try
        {
            await _cachedCustomerLock.WaitAsync(cancellationToken);

            _cachedCustomer = mapper.MapTo(
                await customerServiceClient.GetAsync(
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

    public async Task SubmitFeedbackAsync(WorkspaceMember workspaceMember, string feedback, CancellationToken cancellationToken) =>
        await customerServiceClient.SubmitFeedbackAsync(
            new SubmitFeedbackInput { Id = randomHelper.Generate(), Channel = FeedbackChannel.Slack, Feedback = feedback },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

    public async Task<Customer> AddPreferredLocationAsync(WorkspaceMember workspaceMember, string locationId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.AddPreferredLocationAsync(
                new AddPreferredLocationInput { LocationId = locationId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> RemovePreferredLocationAsync(
        WorkspaceMember workspaceMember,
        string locationId,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.RemovePreferredLocationAsync(
                new RemovePreferredLocationInput { LocationId = locationId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> AddPreferredTeamAsync(WorkspaceMember workspaceMember, string teamId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.AddPreferredTeamAsync(
                new AddPreferredTeamInput { TeamId = teamId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> RemovePreferredTeamAsync(WorkspaceMember workspaceMember, string teamId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.RemovePreferredTeamAsync(
                new RemovePreferredTeamInput { TeamId = teamId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> AddPreferredOrganizationTagAsync(
        WorkspaceMember workspaceMember,
        string organizationTagId,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.AddPreferredOrganizationTagAsync(
                new AddPreferredOrganizationTagInput { OrganizationTagId = organizationTagId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> RemovePreferredOrganizationTagAsync(
        WorkspaceMember workspaceMember,
        string organizationTagId,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.RemovePreferredOrganizationTagAsync(
                new RemovePreferredOrganizationTagInput { OrganizationTagId = organizationTagId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> AddPreferredResourceAsync(
        WorkspaceMember workspaceMember,
        string resourceId,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.AddPreferredResourceAsync(
                new AddPreferredResourceInput { ResourceId = resourceId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

    public async Task<Customer> RemovePreferredResourceAsync(
        WorkspaceMember workspaceMember,
        string resourceId,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.RemovePreferredResourceAsync(
                new RemovePreferredResourceInput { ResourceId = resourceId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

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
        }

        _disposed = true;
    }
}
