using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Shared;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using LocationConfiguration = Api.Shared.Clients.Configurations.Grpc.LocationConfiguration;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection;
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
    LocationConfiguration locationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient,
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    IRandomHelper randomHelper,
    IAdminCustomerService adminCustomerService) : IWorkspaceMemberService
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
        var customerExistenceResult = await adminCustomerService.AnyCustomerExistByVerifiableTokenAsync(workspaceMember.Id, cancellationToken);
        if (customerExistenceResult.Exists)
        {
            customerId = customerExistenceResult.Customer.Id;
            if (string.IsNullOrWhiteSpace(customerExistenceResult.Customer.DefaultOrganization?.Id))
            {
                _ = await adminCustomerService.SetDefaultOrganizationAsync(
                    customerExistenceResult.Customer.Id,
                    workspace.Organization.Id,
                    cancellationToken);
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

            customerExistenceResult = await adminCustomerService.AnyCustomerExistByEmailAsync(workspaceMember.Email, cancellationToken);
            if (customerExistenceResult.Exists)
            {
                customerId = customerExistenceResult.Customer.Id;
                _ = await adminCustomerService.AddIdentityAsync(workspaceMember, customerExistenceResult.Customer.Id, cancellationToken);

                if (string.IsNullOrWhiteSpace(customerExistenceResult.Customer.DefaultOrganization?.Id))
                {
                    _ = await adminCustomerService.SetDefaultOrganizationAsync(
                        customerExistenceResult.Customer.Id,
                        workspace.Organization.Id,
                        cancellationToken);
                }

                if (getLocationsResponse.TotalCount == 1)
                {
                    _ = await adminCustomerService.AddPreferredLocationAsync(
                        customerExistenceResult.Customer.Id,
                        getLocationsResponse.Edges.First().Node.Id,
                        cancellationToken);
                }
            }
            else
            {
                customerId = randomHelper.Generate();
                _ = await adminCustomerService.AddAsync(
                    workspaceMember,
                    customerId,
                    workspace.Organization.Id,
                    getLocationsResponse.TotalCount == 1 ? [getLocationsResponse.Edges.First().Node.Id] : [],
                    cancellationToken);
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
                Member = new OrganizationMember { Id = randomHelper.Generate(), CustomerId = customerId, Role = role }
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
    }
}
