using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Enterprise.Shared.Grpc;
using Slack.Shared.Database.Entities;
using Slack.Shared.Mappers;
using Customer = Slack.Shared.Models.Customer;
using CustomerConfiguration = Api.Shared.Clients.Configurations.Grpc.CustomerConfiguration;

namespace Slack.Shared.Services.CrossDomains;

public interface IAdminCustomerService
{
    Task<Customer> GetAsync(string customerId, CancellationToken cancellationToken);

    Task<Customer> AddAsync(
        WorkspaceMember workspaceMember,
        string customerId,
        string defaultOrganizationId,
        ICollection<string> preferredLocationIds,
        CancellationToken cancellationToken);

    Task<Customer> AddIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken);
    Task<Customer> UpdateIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken);
    Task<(bool Exists, Customer Customer)> AnyCustomerExistByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<(bool Exists, Customer Customer)> AnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Customer> SetDefaultOrganizationAsync(string customerId, string organizationId, CancellationToken cancellationToken);
    Task<Customer> AddPreferredLocationAsync(string customerId, string locationId, CancellationToken cancellationToken);
}

public class AdminCustomerService(
    CustomerConfiguration customerConfiguration,
    Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService.CustomerServiceClient customerServiceClient,
    IMapper mapper)
    : IAdminCustomerService
{
    public async Task<Customer> GetAsync(string customerId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.Admin_GetAsync(
                new Admin_GetInput { CustomerId = customerId },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

    public async Task<Customer> AddAsync(
        WorkspaceMember workspaceMember,
        string customerId,
        string defaultOrganizationId,
        ICollection<string> preferredLocationIds,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.Admin_AddAsync(
                mapper.MapTo(workspaceMember, customerId, defaultOrganizationId, preferredLocationIds),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

    public async Task<Customer> AddIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.Admin_AddIdentityAsync(
                mapper.MapToAddIdentityInput(workspaceMember, customerId),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

    public async Task<Customer> UpdateIdentityAsync(WorkspaceMember workspaceMember, string customerId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.Admin_UpdateIdentityAsync(
                mapper.MapToUpdateIdentityInput(workspaceMember, customerId),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

    public async Task<(bool, Customer)> AnyCustomerExistByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        var result = await customerServiceClient.Admin_AnyCustomerExistByVerifiableTokenAsync(
            new Admin_AnyCustomerExistByVerifiableTokenInput { VerifiableToken = verifiableToken },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return (result.Exist, mapper.MapTo(result.Customer));
    }

    public async Task<(bool, Customer)> AnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var result = await customerServiceClient.Admin_AnyCustomerExistByEmailAsync(
            new Admin_AnyCustomerExistByEmailInput { Email = email },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return (result.Exist, mapper.MapTo(result.Customer));
    }

    public async Task<Customer> SetDefaultOrganizationAsync(string customerId, string organizationId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                new Admin_SetDefaultOrganizationInput { CustomerId = customerId, OrganizationId = organizationId },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

    public async Task<Customer> AddPreferredLocationAsync(string customerId, string locationId, CancellationToken cancellationToken) =>
        mapper.MapTo(
            await customerServiceClient.Admin_AddPreferredLocationAsync(
                new Admin_AddPreferredLocationInput { CustomerId = customerId, LocationId = locationId },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));
}
