using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;
using Customer = Slack.Shared.Models.Customer;
using CustomerConfiguration = Api.Shared.Clients.Configurations.Grpc.CustomerConfiguration;

namespace Slack.Shared.Services.CrossDomains;

public interface ICustomerService
{
    Task<Customer> AdminGetAsync(string customerId, CancellationToken cancellationToken);

    Task<Customer> AdminAddAsync(
        WorkspaceMember workspaceMember,
        string customerId,
        string defaultOrganizationId,
        IReadOnlyList<string> preferredLocationIds,
        CancellationToken cancellationToken);

    Task<Customer> AdminAddIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken);
    Task<Customer> AdminUpdateIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken);
    Task<(bool Exists, Customer? Customer)> AdminAnyCustomerExistByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<(bool Exists, Customer? Customer)> AdminAnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken);
    Task SubmitFeedbackAsync(string workspaceMemberId, string feedback, CancellationToken cancellationToken);
    Task<Customer> AdminSetDefaultOrganizationAsync(string customerId, string organizationId, CancellationToken cancellationToken);
    Task<Customer> AdminAddPreferredLocationAsync(string customerId, string locationId, CancellationToken cancellationToken);
    Task<Customer> GetAsync(string workspaceMemberId, CancellationToken cancellationToken);
    Task<Customer> GetByIdAsync(string workspaceMemberId, string customerId, CancellationToken cancellationToken);
    Task<Customer> AddPreferredLocationAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken);
    Task<Customer> RemovePreferredLocationAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken);
    Task<Customer> AddPreferredOrganizationTagAsync(string workspaceMemberId, string organizationTagId, CancellationToken cancellationToken);
    Task<Customer> RemovePreferredOrganizationTagAsync(string workspaceMemberId, string organizationTagId, CancellationToken cancellationToken);
    Task<Customer> AddPreferredResourceAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken);
    Task<Customer> RemovePreferredResourceAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken);
}

public class CustomerService(
    ApplicationConfiguration applicationConfiguration,
    CustomerConfiguration customerConfiguration,
    Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService.CustomerServiceClient customerServiceClient,
    IMapper mapper,
    IRandomHelper randomHelper,
    IMemoryCache memoryCache,
    IOrganizationTagService organizationTagService,
    ILocationService locationService,
    ILocationResourceService locationResourceService,
    IOrganizationService organizationService)
    : ICustomerService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<Customer> AdminGetAsync(string customerId, CancellationToken cancellationToken)
    {
        var customer = await memoryCache.GetOrCreateAsync(
            CreateKeyById(customerId),
            async _ => mapper.MapTo(
                await customerServiceClient.Admin_GetAsync(
                    new Admin_GetInput { CustomerId = customerId },
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken))!,
            _cacheEntryOptions);

        var locations =
            await Task.WhenAll(customer!.PreferredLocations.Select(item => locationService.AdminGetAsync(item.Id, cancellationToken)));

        var resources =
            await Task.WhenAll(customer.PreferredResources.Select(item => locationResourceService.AdminGetAsync(item.Id, cancellationToken)));

        var organizationTags =
            await Task.WhenAll(customer.PreferredOrganizationTags.Select(item => organizationTagService.AdminGetAsync(item.Id, cancellationToken)));

        if (customer.DefaultOrganization is not null)
        {
            customer.DefaultOrganization = await organizationService.AdminGetAsync(customer.DefaultOrganization.Id, cancellationToken);
        }

        customer.PreferredLocations = customer.PreferredLocations
            .Select(location => locations.FirstOrDefault(item => item.Id == location.Id) ?? location)
            .ToList();

        customer.PreferredResources = customer.PreferredResources
            .Select(resource => resources.FirstOrDefault(item => item.Id == resource.Id) ?? resource)
            .ToList();

        customer.PreferredOrganizationTags = customer.PreferredOrganizationTags
            .Select(organizationTag => organizationTags.FirstOrDefault(item => item.Id == organizationTag.Id) ?? organizationTag)
            .ToList();

        return customer;
    }

    public async Task<Customer> AdminAddAsync(
        WorkspaceMember workspaceMember,
        string customerId,
        string defaultOrganizationId,
        IReadOnlyList<string> preferredLocationIds,
        CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.Admin_AddAsync(
                mapper.MapTo(workspaceMember, customerId, defaultOrganizationId, preferredLocationIds),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> AdminAddIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.Admin_AddIdentityAsync(
                mapper.MapToAddIdentityInput(workspaceMember, customerId),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> AdminUpdateIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.Admin_UpdateIdentityAsync(
                mapper.MapToUpdateIdentityInput(workspaceMember, customerId),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<(bool, Customer?)> AdminAnyCustomerExistByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        var result = await customerServiceClient.Admin_AnyCustomerExistByVerifiableTokenAsync(
            new Admin_AnyCustomerExistByVerifiableTokenInput { VerifiableToken = verifiableToken },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var customer = mapper.MapTo(result.Customer);
        if (customer is not null)
        {
            Cache([customer]);
        }

        return (result.Exist, customer);
    }

    public async Task<(bool, Customer?)> AdminAnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var result = await customerServiceClient.Admin_AnyCustomerExistByEmailAsync(
            new Admin_AnyCustomerExistByEmailInput { Email = email },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var customer = mapper.MapTo(result.Customer);
        if (customer is not null)
        {
            Cache([customer]);
        }

        return (result.Exist, customer);
    }

    public async Task<Customer> AdminSetDefaultOrganizationAsync(string customerId, string organizationId, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                new Admin_SetDefaultOrganizationInput { CustomerId = customerId, OrganizationId = organizationId },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> AdminAddPreferredLocationAsync(string customerId, string locationId, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.Admin_AddPreferredLocationAsync(
                new Admin_AddPreferredLocationInput { CustomerId = customerId, LocationId = locationId },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task SubmitFeedbackAsync(string workspaceMemberId, string feedback, CancellationToken cancellationToken) =>
        await customerServiceClient.SubmitFeedbackAsync(
            new SubmitFeedbackInput { Id = randomHelper.Generate(), Channel = FeedbackChannel.Slack, Feedback = feedback },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

    public async Task<Customer> GetAsync(string workspaceMemberId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyByVerifiableToken(workspaceMemberId),
            async _ => mapper.MapTo(
                await customerServiceClient.GetAsync(
                    new GetInput(),
                    customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Customer> GetByIdAsync(string workspaceMemberId, string customerId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(customerId),
            async _ => mapper.MapTo(
                await customerServiceClient.GetByIdAsync(
                    new GetByIdInput { CustomerId = customerId },
                    customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Customer> AddPreferredLocationAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.AddPreferredLocationAsync(
                new AddPreferredLocationInput { LocationId = locationId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> RemovePreferredLocationAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.RemovePreferredLocationAsync(
                new RemovePreferredLocationInput { LocationId = locationId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> AddPreferredOrganizationTagAsync(
        string workspaceMemberId,
        string organizationTagId,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.AddPreferredOrganizationTagAsync(
                new AddPreferredOrganizationTagInput { OrganizationTagId = organizationTagId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken))!;

    public async Task<Customer> RemovePreferredOrganizationTagAsync(
        string workspaceMemberId,
        string organizationTagId,
        CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.RemovePreferredOrganizationTagAsync(
                new RemovePreferredOrganizationTagInput { OrganizationTagId = organizationTagId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> AddPreferredResourceAsync(
        string workspaceMemberId,
        string resourceId,
        CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.AddPreferredResourceAsync(
                new AddPreferredResourceInput { ResourceId = resourceId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    public async Task<Customer> RemovePreferredResourceAsync(
        string workspaceMemberId,
        string resourceId,
        CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(
            await customerServiceClient.RemovePreferredResourceAsync(
                new RemovePreferredResourceInput { ResourceId = resourceId },
                customerConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken))!;

        Cache([customer]);

        return customer;
    }

    private void Cache(IReadOnlyList<Customer> customers)
    {
        foreach (var customer in customers)
        {
            var key = CreateKeyById(customer.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, customer, _cacheEntryOptions);

            foreach (var identity in customer.Identities)
            {
                key = CreateKeyByVerifiableToken(identity.Id);

                memoryCache.Remove(key);
                memoryCache.Set(key, customer, _cacheEntryOptions);
            }
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:customer-id:{id}";

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:customer-verifiabletoken:{verifiableToken}";
}
