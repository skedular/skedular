using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using SlackNet;
using Customer = Api.Shared.Services.Grpc.Skedular.Organization.V1.Customer;
using CustomerConfiguration = Api.Shared.Clients.Configurations.Grpc.CustomerConfiguration;
using Icons = Slack.Shared.Constants.Icons;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using Location = Slack.Shared.Database.Entities.Location;
using LocationConfiguration = Api.Shared.Clients.Configurations.Grpc.LocationConfiguration;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection;
using Organization = Slack.Shared.Database.Entities.Organization;
using OrganizationMember = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMember;
using OrganizationMemberStatus = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus;
using Role = Api.Shared.Services.Grpc.Skedular.Organization.V1.Role;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Shared.Services;

public interface IWorkspaceMemberService
{
    Task RefreshWorkspaceMembersAsync(string workspaceId, CancellationToken cancellationToken);
    Task UpdateWorkspaceMemberProfileStatusAsync(string workspaceMemberId, CancellationToken cancellationToken);
    string GetMentionedCustomerNameInSlackFormat(Workspace workspace, ICollection<string> identities, Models.Customer customer);
}

public class WorkspaceMemberService(
    BookingConfiguration bookingConfiguration,
    CustomerConfiguration customerConfiguration,
    LocationConfiguration locationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    BookingService.BookingServiceClient bookingServiceClient,
    CustomerService.CustomerServiceClient customerServiceClient,
    LocationService.LocationServiceClient locationServiceClient,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : IWorkspaceMemberService
{
    public async Task RefreshWorkspaceMembersAsync(string workspaceId, CancellationToken cancellationToken)
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

        var workspaceMembers = await repositoryFactory.WorkspaceMemberRepository.GetByWorkspaceIdAsync(
            workspaceId,
            cancellationToken);
        var itemsToRemove = workspaceMembers
            .Where(workspaceMember => users.All(item => item.Id != workspaceMember.Id))
            .ToList();
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

        existingWorkspace.MembersLastRefreshedAt = timeProvider.GetUtcNow();
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
            new BookingOrderInput
            {
                Direction = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrderDirection.Ascending, Field = BookingOrderField.From
            }
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

    public string GetMentionedCustomerNameInSlackFormat(Workspace workspace, ICollection<string> identities, Models.Customer customer)
    {
        var workspaceMember = workspace.WorkspaceMembers.FirstOrDefault(item => identities.Contains(item.Id));
        return workspaceMember is null ? customer.ToDisplayableName() : $"<@{workspaceMember.Id}>";
    }

    private async Task SyncCustomersAndOrganizationMembersAsync(Database.Entities.Workspace workspace, CancellationToken cancellationToken)
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

        var customerIdsWorkspaceMembersPair = new List<(string, WorkspaceMember)>();

        foreach (var workspaceMember in workspace.WorkspaceMembers)
        {
            var anyCustomerExistByVerifiableTokenResponse = await customerServiceClient.Admin_AnyCustomerExistByVerifiableTokenAsync(
                new Admin_AnyCustomerExistByVerifiableTokenInput { VerifiableToken = workspaceMember.Id },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
            if (anyCustomerExistByVerifiableTokenResponse.Exist)
            {
                customerIdsWorkspaceMembersPair.Add(
                    (anyCustomerExistByVerifiableTokenResponse.Customer.Id, workspaceMember));

                await customerServiceClient.Admin_UpdateIdentityAsync(
                    mapper.MapToUpdateIdentityInput(
                        workspaceMember,
                        anyCustomerExistByVerifiableTokenResponse.Customer.Id),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(anyCustomerExistByVerifiableTokenResponse.Customer.DefaultOrganization?.Id))
                {
                    await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                        new Admin_SetDefaultOrganizationInput
                        {
                            OrganizationId = workspace.Organization.Id, CustomerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                if (getLocationsResponse.TotalCount == 1)
                {
                    await customerServiceClient.Admin_AddPreferredLocationAsync(
                        new Admin_AddPreferredLocationInput
                        {
                            LocationId = getLocationsResponse.Edges.First().Node.Id,
                            CustomerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                continue;
            }

            var anyCustomerExistByEmailTokenResponse = await customerServiceClient.Admin_AnyCustomerExistByEmailAsync(
                new Admin_AnyCustomerExistByEmailInput { Email = workspaceMember.Email },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
            if (anyCustomerExistByEmailTokenResponse.Exist)
            {
                customerIdsWorkspaceMembersPair.Add((anyCustomerExistByEmailTokenResponse.Customer.Id, workspaceMember));
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

                continue;
            }

            var customerId = randomHelper.Generate();
            customerIdsWorkspaceMembersPair.Add((customerId, workspaceMember));
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

        await customerIdsWorkspaceMembersPair.Select(customerIdWorkspaceMemberPair =>
        {
            var customerId = customerIdWorkspaceMemberPair.Item1;
            var workspaceMember = customerIdWorkspaceMemberPair.Item2;
            var organizationMember =
                workspace.Organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customerId);

            if (organizationMember is null)
            {
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

                return new OrganizationMember
                {
                    Id = randomHelper.Generate(), Customer = new Customer { Id = customerId }, Role = role, IsOrganizationOnboardingDone = true
                };
            }

            return new OrganizationMember
            {
                Id = organizationMember.Id,
                Customer = new Customer { Id = customerId },
                Role = organizationMember.Role switch
                {
                    OrganizationMemberRoleConstants.Owner => Role.Owner,
                    OrganizationMemberRoleConstants.Administrator => Role.Administrator,
                    OrganizationMemberRoleConstants.Member => Role.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Status = organizationMember.Status switch
                {
                    OrganizationMemberStatusConstants.Active => OrganizationMemberStatus.Active,
                    OrganizationMemberStatusConstants.Inactive => OrganizationMemberStatus.Inactive,
                    _ => throw new ArgumentOutOfRangeException()
                },
                IsOrganizationOnboardingDone = true
            };
        }).ForEachAsync(async (member, ct) =>
        {
            await organizationServiceClient.Admin_AddMemberAsync(
                new Admin_AddMemberInput { Id = workspace.Organization.Id, Member = member },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: ct);
        }, cancellationToken);
    }
}
