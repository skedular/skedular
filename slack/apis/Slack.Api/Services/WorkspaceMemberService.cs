using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Shared;
using Slack.Shared.Repositories;
using Customer = Api.Shared.Services.Grpc.Skedular.Organization.V1.Customer;
using LocationConfiguration = Api.Shared.Clients.Configurations.Grpc.LocationConfiguration;
using CustomerConfiguration = Api.Shared.Clients.Configurations.Grpc.CustomerConfiguration;
using Location = Slack.Shared.Database.Entities.Location;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection;
using Organization = Slack.Shared.Database.Entities.Organization;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using Role = Api.Shared.Services.Grpc.Skedular.Organization.V1.Role;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Api.Services;

public interface IWorkspaceMemberService
{
    Task<(WorkspaceMember, string)> EnsureCustomerResourcesAllExistAsync(
        Workspace workspace,
        string workspaceMemberId,
        CancellationToken cancellationToken);
}

public class WorkspaceMemberService(
    IRepositoryFactory repositoryFactory,
    CustomerConfiguration customerConfiguration,
    LocationConfiguration locationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    global::Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService.CustomerServiceClient customerServiceClient,
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient,
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient
        organizationServiceClient,
    IMapper mapper,
    IRandomHelper randomHelper) : IWorkspaceMemberService
{
    public async Task<(WorkspaceMember, string)> EnsureCustomerResourcesAllExistAsync(
        Workspace workspace,
        string workspaceMemberId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(workspaceMemberId, cancellationToken);
        var workspaceMember = await EnsureWorkspaceMemberExistAsync(workspace, workspaceMemberId, cancellationToken);
        if (customer is not null)
        {
            await EnsureOrganizationMemberExistsAsync(workspace, workspaceMember, customer.Id, cancellationToken);
            return (workspaceMember, customer.Id);
        }

        var customerId = await EnsureCustomerExistAsync(workspace, workspaceMember, cancellationToken);
        await EnsureOrganizationMemberExistsAsync(workspace, workspaceMember, customerId, cancellationToken);

        return (workspaceMember, customerId);
    }

    private async Task<WorkspaceMember> EnsureWorkspaceMemberExistAsync(
        Workspace workspace,
        string workspaceMemberId,
        CancellationToken cancellationToken)
    {
        var workspaceMember = workspace.WorkspaceMembers.FirstOrDefault(item => item.Id == workspaceMemberId);
        if (workspaceMember is not null)
        {
            return workspaceMember;
        }

        var user = await workspace.GetApiClient().Users.Info(workspaceMemberId, true, cancellationToken);
        if (!user.IsAcceptableWorkspaceMemberType())
        {
            throw new SlackWorkspaceMemberTypeNotSupported();
        }

        workspaceMember = repositoryFactory.WorkspaceMemberRepository.Add(mapper.MapToEntity(user, workspace));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return workspaceMember;
    }

    private async Task<string> EnsureCustomerExistAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        string customerId;
        var anyCustomerExistByVerifiableTokenResponse = await customerServiceClient.Admin_AnyCustomerExistByVerifiableTokenAsync(
            new Admin_AnyCustomerExistByVerifiableTokenInput { VerifiableToken = workspaceMember.Id },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        if (anyCustomerExistByVerifiableTokenResponse.Exist)
        {
            customerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id;
            if (string.IsNullOrWhiteSpace(
                    anyCustomerExistByVerifiableTokenResponse.Customer.DefaultOrganization?.Id))
            {
                await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                    new Admin_SetDefaultOrganizationInput
                    {
                        OrganizationId = workspace.Organization.Id, CustomerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id
                    },
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);
            }
        }
        else
        {
            var getPaginatedLocationsInput = new Admin_GetPaginatedLocationsInput
            {
                First = ((int?)null).ToNullInt(),
                Last = ((int?)null).ToNullInt(),
                Where = new LocationWhereInput { OrganizationId = workspace.Organization.Id }
            };
            getPaginatedLocationsInput.OrderBy.AddRange([
                new LocationOrderInput { Direction = OrderDirection.Ascending, Field = LocationOrderField.Name }
            ]);
            var getLocationsResponse = await locationServiceClient.Admin_GetPaginatedLocationsAsync(
                getPaginatedLocationsInput,
                locationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);

            var anyCustomerExistByEmailTokenResponse =
                await customerServiceClient.Admin_AnyCustomerExistByEmailAsync(
                    new Admin_AnyCustomerExistByEmailInput { Email = workspaceMember.Email },
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);
            if (anyCustomerExistByEmailTokenResponse.Exist)
            {
                customerId = anyCustomerExistByEmailTokenResponse.Customer.Id;
                await customerServiceClient.Admin_AddIdentityAsync(
                    mapper.MapTo(workspaceMember, anyCustomerExistByEmailTokenResponse.Customer.Id),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(
                        anyCustomerExistByEmailTokenResponse.Customer.DefaultOrganization?.Id))
                {
                    await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                        new Admin_SetDefaultOrganizationInput
                        {
                            OrganizationId = workspace.Organization.Id, CustomerId = anyCustomerExistByEmailTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                if (getLocationsResponse.TotalCount == 1)
                {
                    await customerServiceClient.Admin_AddPreferredLocationAsync(
                        new Admin_AddPreferredLocationInput
                        {
                            LocationId = getLocationsResponse.Edges.First().Node.Id, CustomerId = anyCustomerExistByEmailTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                customerId = randomHelper.Generate();
                await customerServiceClient.Admin_AddAsync(
                    mapper.MapTo(
                        workspaceMember,
                        customerId,
                        new Organization { Id = workspace.Organization.Id },
                        getLocationsResponse.TotalCount == 1
                            ? [new Location { Id = getLocationsResponse.Edges.First().Node.Id }]
                            : []),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);
            }
        }

        return customerId;
    }

    private async Task EnsureOrganizationMemberExistsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string customerId,
        CancellationToken cancellationToken)
    {
        if (workspace.Organization.OrganizationMembers.Any(item => item.Customer.Id == customerId))
        {
            return;
        }

        Role role;
        if (workspaceMember.IsPrimaryOwner || workspaceMember.IsOwner)
        {
            role = Role.Owner;
        }
        else if (workspaceMember.IsAdmin)
        {
            role = Role.Administrator;
        }
        else
        {
            role = Role.Member;
        }

        await organizationServiceClient.Admin_AddMemberAsync(
            new Admin_AddMemberInput
            {
                Id = workspace.Organization.Id,
                Member = new OrganizationMember
                {
                    Id = randomHelper.Generate(),
                    Customer =
                        new Customer { Id = customerId },
                    Role = role
                }
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
    }
}
