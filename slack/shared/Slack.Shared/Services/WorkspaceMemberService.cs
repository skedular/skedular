using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using Customer = Slack.Shared.Models.Customer;
using Icons = Slack.Shared.Constants.Icons;
using Organization = Slack.Shared.Models.Organization;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Shared.Services;

public interface IWorkspaceMemberService
{
    Task ReSyncWorkspaceMembersAsync(string workspaceId, CancellationToken cancellationToken);
    Task UpdateWorkspaceMemberProfileStatusAsync(string workspaceMemberId, CancellationToken cancellationToken);
    string GetMentionedCustomerNameInSlackFormat(Workspace workspace, ICollection<string> identities, Customer customer);
}

public class WorkspaceMemberService(
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    ICustomerService customerService,
    IOrganizationMemberService organizationMemberService,
    ILocationService locationService) : IWorkspaceMemberService
{
    public async Task ReSyncWorkspaceMembersAsync(string workspaceId, CancellationToken cancellationToken)
    {
        var existingWorkspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (existingWorkspace is null)
        {
            return;
        }

        var nextCursor = string.Empty;
        var users = new List<User>();

        do
        {
            var response = await existingWorkspace.GetApiClient().Users.List(nextCursor, true, 100, cancellationToken);
            users.AddRange(response.Members.Where(item => item.IsAcceptableWorkspaceMemberType()));
            nextCursor = response.ResponseMetadata.NextCursor;
        } while (!string.IsNullOrWhiteSpace(nextCursor));

        var workspaceMembers = await repositoryFactory.WorkspaceMemberRepository.GetByWorkspaceIdAsync(workspaceId, cancellationToken);
        var itemsToRemove = workspaceMembers.Where(workspaceMember => users.All(item => item.Id != workspaceMember.Id)).ToList();
        var updatedItems = workspaceMembers
            .Where(workspaceMember => users.Any(item => item.Id == workspaceMember.Id))
            .Select(workspaceMember =>
            {
                var updatedWorkspaceMember = mapper.MergeToEntity(
                    users.First(item => item.Id == workspaceMember.Id),
                    workspaceMember,
                    existingWorkspace);
                updatedWorkspaceMember.DeletedAt = null;
                return repositoryFactory.WorkspaceMemberRepository.Update(updatedWorkspaceMember);
            }).ToList();
        var addedItems = users.Where(user => workspaceMembers.All(item => item.Id != user.Id))
            .Select(user => repositoryFactory.WorkspaceMemberRepository.Add(mapper.MapToEntity(user, existingWorkspace)))
            .ToList();

        repositoryFactory.WorkspaceMemberRepository.RemoveRange(itemsToRemove);
        existingWorkspace.WorkspaceMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        repositoryFactory.WorkspaceRepository.Update(existingWorkspace);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await SyncCustomersAndOrganizationMembersAsync(existingWorkspace, cancellationToken);
    }

    public async Task UpdateWorkspaceMemberProfileStatusAsync(string workspaceMemberId, CancellationToken cancellationToken)
    {
        var workspaceMemberEntity = await repositoryFactory.WorkspaceMemberRepository.GetByIdAsync(workspaceMemberId, cancellationToken);
        if (workspaceMemberEntity is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        if (workspaceMemberEntity.LastProfileStatusUpdatedAt is not null &&
            (now - workspaceMemberEntity.LastProfileStatusUpdatedAt.Value).TotalHours <= 24)
        {
            return;
        }

        workspaceMemberEntity.LastProfileStatusUpdatedAt = now;
        repositoryFactory.WorkspaceMemberRepository.Update(workspaceMemberEntity);

        var customerEntity = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(workspaceMemberId, cancellationToken);
        if (customerEntity is null)
        {
            return;
        }

        var workspace = workspaceMemberEntity.Workspace;
        var slackApiClient = workspace.GetUserApiClient();
        var userProfile = await slackApiClient.UserProfile.Get(true, workspaceMemberEntity.Id, cancellationToken);
        if (!string.IsNullOrWhiteSpace(userProfile.StatusText) && !string.IsNullOrWhiteSpace(userProfile.StatusEmoji))
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var from = now.StartOfDay();
        var until = from.EndOfDay();
        var getPaginatedBookingsInput = new GetPaginatedBookingsInput
        {
            After = string.Empty,
            First = ((int?)null).ToNullInt(),
            Before = string.Empty,
            Last = ((int?)null).ToNullInt(),
            Where = new BookingWhereInput { FromGte = from.ToTimestamp(), FromLte = until.ToTimestamp(), IncludeMineOnly = true }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        getPaginatedBookingsInput.OrderBy.AddRange([
            new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }
        ]);
        var bookingConnection = await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberEntity.Id),
            cancellationToken: cancellationToken);

        if (bookingConnection.TotalCount != 0)
        {
            var locations = bookingConnection.Edges
                .SelectMany(item => item.Node.InvolvedLocations)
                .Aggregate(string.Empty, (acc, location) => $"{acc}, {location.Name.ToSafeString()}")
                .Trim(',')
                .Trim();
            var location = string.IsNullOrWhiteSpace(locations) ? "Unknown" : locations;
            userProfile.StatusText = string.IsNullOrWhiteSpace(userProfile.StatusText)
                ? $"Work from '{location}'"
                : userProfile.StatusText;
            userProfile.StatusEmoji = string.IsNullOrWhiteSpace(userProfile.StatusEmoji)
                ? Icons.Office
                : userProfile.StatusEmoji;
            userProfile.StatusExpiration = now.StartOfDay().EndOfDay().ToUnixTimeSeconds();
            await slackApiClient.UserProfile.Set(userProfile, workspaceMemberEntity.Id, cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public string GetMentionedCustomerNameInSlackFormat(Workspace workspace, ICollection<string> identities, Customer customer)
    {
        var workspaceMember = workspace.WorkspaceMembers.FirstOrDefault(item => identities.Contains(item.Id));
        return workspaceMember is null ? customer.DisplayableName : $"<@{workspaceMember.Id}>";
    }

    private async Task SyncCustomersAndOrganizationMembersAsync(Database.Entities.Workspace workspace, CancellationToken cancellationToken)
    {
        var locations = await locationService.AdminGetAllLocationsAsync(workspace.Organization.Id, cancellationToken);
        var customerIdsWorkspaceMembersPair = new List<(string, WorkspaceMember)>();

        foreach (var workspaceMember in workspace.WorkspaceMembers)
        {
            var customerExistenceResult =
                await customerService.AdminAnyCustomerExistByVerifiableTokenAsync(workspaceMember.Id, cancellationToken);
            if (customerExistenceResult.Exists)
            {
                customerIdsWorkspaceMembersPair.Add((customerExistenceResult.Customer!.Id, workspaceMember));

                _ = await customerService.AdminUpdateIdentityAsync(workspaceMember, customerExistenceResult.Customer.Id, cancellationToken);

                if (string.IsNullOrWhiteSpace(customerExistenceResult.Customer.DefaultOrganization?.Id))
                {
                    _ = await customerService.AdminSetDefaultOrganizationAsync(
                        customerExistenceResult.Customer.Id,
                        workspace.Organization.Id,
                        cancellationToken);
                }

                if (locations.Count == 1)
                {
                    _ = await customerService.AdminAddPreferredLocationAsync(
                        customerExistenceResult.Customer.Id,
                        locations.First().Id,
                        cancellationToken);
                }

                continue;
            }

            customerExistenceResult = await customerService.AdminAnyCustomerExistByEmailAsync(workspaceMember.Email, cancellationToken);
            if (customerExistenceResult.Exists)
            {
                customerIdsWorkspaceMembersPair.Add((customerExistenceResult.Customer!.Id, workspaceMember));
                _ = await customerService.AdminAddIdentityAsync(
                    workspaceMember,
                    customerExistenceResult.Customer.Id,
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(customerExistenceResult.Customer.DefaultOrganization?.Id))
                {
                    _ = await customerService.AdminSetDefaultOrganizationAsync(
                        customerExistenceResult.Customer.Id,
                        workspace.Organization.Id,
                        cancellationToken);
                }

                if (locations.Count == 1)
                {
                    _ = await customerService.AdminAddPreferredLocationAsync(
                        customerExistenceResult.Customer.Id,
                        locations.First().Id,
                        cancellationToken);
                }

                continue;
            }

            var customerId = randomHelper.Generate();
            customerIdsWorkspaceMembersPair.Add((customerId, workspaceMember));

            _ = await customerService.AdminAddAsync(
                workspaceMember,
                customerId,
                workspace.Organization.Id,
                locations.Count == 1 ? [locations.First().Id] : [],
                cancellationToken);
        }

        await customerIdsWorkspaceMembersPair.Select(customerIdWorkspaceMemberPair =>
        {
            var customerId = customerIdWorkspaceMemberPair.Item1;
            var workspaceMember = customerIdWorkspaceMemberPair.Item2;
            var organizationMember =
                workspace.Organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customerId);

            if (organizationMember is null)
            {
                OrganizationMemberRole role;
                if (workspaceMember.IsPrimaryOwner || workspaceMember.IsOwner)
                {
                    role = OrganizationMemberRole.Owner;
                }
                else if (workspaceMember.IsAdmin)
                {
                    role = OrganizationMemberRole.Administrator;
                }
                else
                {
                    role = OrganizationMemberRole.Member;
                }

                return new OrganizationMember
                {
                    Id = randomHelper.Generate(),
                    Customer = new Customer { Id = customerId },
                    Role = role,
                    IsOrganizationOnboardingDone = true,
                    Organization = new Organization { Id = workspace.Organization.Id }
                };
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(organizationMember.Role);
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationMember.Status);

            return new OrganizationMember
            {
                Id = organizationMember.Id,
                Customer = new Customer { Id = customerId },
                Role = organizationMember.Role.ToOrganizationMemberRole(),
                Status = organizationMember.Status.ToOrganizationMemberStatus(),
                IsOrganizationOnboardingDone = true,
                Organization = new Organization { Id = workspace.Organization.Id }
            };
        }).ForEachAsync(async (member, ct) => await organizationMemberService.AdminAddAsync(member, ct), cancellationToken);
    }
}
