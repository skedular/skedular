using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Team.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Admin_GetInput = Api.Shared.Grpc.Skedular.Team.Core.V1.Admin_GetInput;
using GetInput = Api.Shared.Grpc.Skedular.Team.Core.V1.GetInput;
using OrderDirection = Api.Shared.Grpc.Skedular.Team.Core.V1.OrderDirection;
using OrganizationMemberRole = Api.Shared.Services.Models.OrganizationMemberRole;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;
using Team = Slack.Shared.Models.Team;
using TeamEdge = Slack.Shared.Models.TeamEdge;
using TeamMemberStatus = Api.Shared.Services.Models.TeamMemberStatus;

namespace Slack.Shared.Services.CrossDomains;

public interface ITeamService
{
    Task<Team> AdminGetAsync(string teamId, CancellationToken cancellationToken);
    Task<Team> GetAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken);
    Task<Team> AddAsync(string workspaceMemberId, Team team, CancellationToken cancellationToken);
    Task<Team> UpdateAsync(string workspaceMemberId, Team team, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken);

    Task<Connection<TeamEdge>> GetPaginatedTeamsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class TeamService(
    ApplicationConfiguration applicationConfiguration,
    TeamConfiguration teamConfiguration,
    Api.Shared.Grpc.Skedular.Team.Core.V1.TeamService.TeamServiceClient teamServiceClient,
    IGrpcMapper grpcMapper,
    IMemoryCache memoryCache,
    ICustomerService customerService,
    ILocationService locationService)
    : ITeamService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(30),
    };

    public async Task<Team> AdminGetAsync(string teamId, CancellationToken cancellationToken)
    {
        var team = await memoryCache.GetOrCreateAsync(
            CreateKeyById(teamId),
            async _ => grpcMapper.MapTo(
                await teamServiceClient.Admin_GetAsync(
                    new Admin_GetInput
                    {
                        Id = teamId,
                    },
                    teamConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions);

        var customers = await Task.WhenAll(
            team!.TeamMembers.Select(item => item.Customer.Id).Select(item => customerService.AdminGetAsync(item, cancellationToken)));

        foreach (var member in team.TeamMembers)
        {
            var matchingCustomer = customers.FirstOrDefault(item => item.Id == member.Customer.Id);
            if (matchingCustomer is not null)
            {
                member.Customer = matchingCustomer;
            }
        }

        if (!string.IsNullOrWhiteSpace(team.PrimaryLocation?.Id))
        {
            team.PrimaryLocation = await locationService.AdminGetAsync(team.PrimaryLocation.Id, cancellationToken);
        }

        return team;
    }

    public async Task<Team> GetAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken)
    {
        var team = await memoryCache.GetOrCreateAsync(
            CreateKeyById(teamId),
            async _ => grpcMapper.MapTo(
                await teamServiceClient.GetAsync(
                    new GetInput
                    {
                        Id = teamId,
                    },
                    teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions);

        var customers = await Task.WhenAll(
            team!.TeamMembers
                .Select(item => item.Customer.Id)
                .Select(item => customerService.GetByIdAsync(workspaceMemberId, item, cancellationToken)));

        foreach (var member in team.TeamMembers)
        {
            var matchingCustomer = customers.FirstOrDefault(item => item.Id == member.Customer.Id);
            if (matchingCustomer is not null)
            {
                member.Customer = matchingCustomer;
            }
        }

        if (!string.IsNullOrWhiteSpace(team.PrimaryLocation?.Id))
        {
            team.PrimaryLocation = await locationService.GetAsync(workspaceMemberId, team.PrimaryLocation.Id, cancellationToken);
        }

        return team;
    }

    public async Task<Team> AddAsync(string workspaceMemberId, Team team, CancellationToken cancellationToken)
    {
        var addInput = new AddInput
        {
            Id = team.Id,
            Name = team.Name,
            About = team.About,
            Timezone = team.Timezone,
            OrganizationId = team.Organization!.Id,
            PrimaryLocationId = team.PrimaryLocation is null ? string.Empty : team.PrimaryLocation.Id.ToSafeString(),
        };

        addInput.Members.AddRange(team.TeamMembers.Select(item => new TeamMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(item.Role), item.Role,
                    $"Unexpected value for {nameof(item.Role)}: {item.Role}. Update enum mapping or caller input."),
            },
            Status = item.Status switch
            {
                TeamMemberStatus.Active => Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Active,
                TeamMemberStatus.Inactive => Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(item.Status), item.Status,
                    $"Unexpected value for {nameof(item.Status)}: {item.Status}. Update enum mapping or caller input."),
            },
            OrganizationMember = new OrganizationMember
            {
                Id = item.OrganizationMember!.Id,
                CustomerId = item.OrganizationMember!.Customer.Id,
                Role = item.OrganizationMember!.Role switch
                {
                    OrganizationMemberRole.Owner => Role.Owner,
                    OrganizationMemberRole.Administrator => Role.Administrator,
                    OrganizationMemberRole.Member => Role.Member,
                    _ => throw new ArgumentOutOfRangeException(nameof(item.OrganizationMember.Role), item.OrganizationMember.Role,
                        $"Unexpected value for {nameof(item.OrganizationMember.Role)}: {item.OrganizationMember.Role}. Update enum mapping or caller input."),
                },
            },
        }));

        var mappedTeam = grpcMapper.MapTo(
            await teamServiceClient.AddAsync(
                addInput,
                teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedTeam]);

        var customers = await Task.WhenAll(
            mappedTeam.TeamMembers
                .Select(item => item.Customer.Id)
                .Select(item => customerService.GetByIdAsync(workspaceMemberId, item, cancellationToken)));

        foreach (var member in mappedTeam.TeamMembers)
        {
            var matchingCustomer = customers.FirstOrDefault(item => item.Id == member.Customer.Id);
            if (matchingCustomer is not null)
            {
                member.Customer = matchingCustomer;
            }
        }

        if (!string.IsNullOrWhiteSpace(mappedTeam.PrimaryLocation?.Id))
        {
            mappedTeam.PrimaryLocation = await locationService.GetAsync(workspaceMemberId, mappedTeam.PrimaryLocation.Id, cancellationToken);
        }

        return mappedTeam;
    }

    public async Task<Team> UpdateAsync(string workspaceMemberId, Team team, CancellationToken cancellationToken)
    {
        var updateInput = new UpdateInput
        {
            Id = team.Id,
            Name = team.Name,
            About = team.About,
            Timezone = team.Timezone,
            OrganizationId = team.Organization!.Id,
            PrimaryLocationId = team.PrimaryLocation is null ? string.Empty : team.PrimaryLocation.Id.ToSafeString(),
        };

        updateInput.Members.AddRange(team.TeamMembers.Select(item => new TeamMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(item.Role), item.Role,
                    $"Unexpected value for {nameof(item.Role)}: {item.Role}. Update enum mapping or caller input."),
            },
            Status = item.Status switch
            {
                TeamMemberStatus.Active => Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Active,
                TeamMemberStatus.Inactive => Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(item.Status), item.Status,
                    $"Unexpected value for {nameof(item.Status)}: {item.Status}. Update enum mapping or caller input."),
            },
            OrganizationMember = new OrganizationMember
            {
                Id = item.OrganizationMember!.Id,
                CustomerId = item.OrganizationMember!.Customer.Id,
                Role = item.OrganizationMember!.Role switch
                {
                    OrganizationMemberRole.Owner => Role.Owner,
                    OrganizationMemberRole.Administrator => Role.Administrator,
                    OrganizationMemberRole.Member => Role.Member,
                    _ => throw new ArgumentOutOfRangeException(nameof(item.OrganizationMember.Role), item.OrganizationMember.Role,
                        $"Unexpected value for {nameof(item.OrganizationMember.Role)}: {item.OrganizationMember.Role}. Update enum mapping or caller input."),
                },
            },
        }));
        updateInput.FieldsToUpdate.AddRange(
        [
            TeamPatchField.Name,
            TeamPatchField.About,
            TeamPatchField.Organization,
            TeamPatchField.Timezone,
            TeamPatchField.Members,
            TeamPatchField.PrimaryLocation,
            TeamPatchField.FeatureImages,
        ]);

        var mappedTeam = grpcMapper.MapTo(
            await teamServiceClient.UpdateAsync(
                updateInput,
                teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        var customers = await Task.WhenAll(
            mappedTeam.TeamMembers
                .Select(item => item.Customer.Id)
                .Select(item => customerService.GetByIdAsync(workspaceMemberId, item, cancellationToken)));

        foreach (var member in mappedTeam.TeamMembers)
        {
            var matchingCustomer = customers.FirstOrDefault(item => item.Id == member.Customer.Id);
            if (matchingCustomer is not null)
            {
                member.Customer = matchingCustomer;
            }
        }

        if (!string.IsNullOrWhiteSpace(mappedTeam.PrimaryLocation?.Id))
        {
            mappedTeam.PrimaryLocation = await locationService.GetAsync(workspaceMemberId, mappedTeam.PrimaryLocation.Id, cancellationToken);
        }

        return mappedTeam;
    }

    public async Task RemoveAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken)
    {
        await teamServiceClient.RemoveAsync(
            new RemoveInput
            {
                Id = teamId,
            },
            teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(teamId);

        memoryCache.Remove(key);
    }

    public async Task<Connection<TeamEdge>> GetPaginatedTeamsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedTeamsInput = new GetPaginatedTeamsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new TeamWhereInput
            {
                OrganizationId = organizationId,
                NameContains = nameContains.ToSafeString(),
            },
        };

        getPaginatedTeamsInput.OrderBy.Add(new TeamOrderInput
        {
            Direction = OrderDirection.Ascending,
            Field = TeamOrderField.Name,
        });

        var connection = await teamServiceClient.GetPaginatedTeamsAsync(
            getPaginatedTeamsInput,
            teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var customers = await Task.WhenAll(
            connection.Edges
                .SelectMany(item => item.Node.Members.Select(member => member.CustomerId))
                .Select(item => customerService.GetByIdAsync(workspaceMemberId, item, cancellationToken)));

        var locations = await Task.WhenAll(
            connection.Edges
                .Where(item => !string.IsNullOrWhiteSpace(item.Node.PrimaryLocationId))
                .Select(item => item.Node.PrimaryLocationId).Select(item => locationService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var result = new Connection<TeamEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage,
            },
            TotalCount = connection.TotalCount,
            Edges =
            [
                .. connection.Edges.Select(item =>
                {
                    var team = grpcMapper.MapTo(item.Node);
                    foreach (var member in team.TeamMembers)
                    {
                        var matchingCustomer = customers.FirstOrDefault(customer => customer.Id == member.Customer.Id);
                        if (matchingCustomer is not null)
                        {
                            member.Customer = matchingCustomer;
                        }
                    }

                    if (team.PrimaryLocation is not null)
                    {
                        var matchingPrimaryLocation = locations.FirstOrDefault(location => location.Id == team.PrimaryLocation.Id);
                        if (matchingPrimaryLocation is not null)
                        {
                            team.PrimaryLocation = matchingPrimaryLocation;
                        }
                    }

                    return new TeamEdge(team, item.Cursor);
                }),
            ],
        };

        Cache([.. result.Edges.Select(item => item.Node)]);

        return result;
    }

    private void Cache(IReadOnlyList<Team> teams)
    {
        foreach (var team in teams)
        {
            var key = CreateKeyById(team.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, team, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:team-id:{id}";
}
