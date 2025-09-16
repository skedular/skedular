using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Admin_GetInput = Api.Shared.Services.Grpc.Skedular.Team.V1.Admin_GetInput;
using GetInput = Api.Shared.Services.Grpc.Skedular.Team.V1.GetInput;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Team.V1.OrderDirection;
using Team = Slack.Shared.Models.Team;

namespace Slack.Shared.Services.CrossDomains;

public interface ITeamService
{
    Task<Team> AdminGetAsync(string teamId, CancellationToken cancellationToken);
    Task<Team> GetAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken);

    Task<(ICollection<Team>, TeamConnection)> GetPaginatedTeamsAsync(
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
    Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService.TeamServiceClient teamServiceClient,
    IMapper mapper,
    HybridCache hybridCache,
    ICustomerService customerService,
    ILocationService locationService)
    : ITeamService
{
    public async Task<Team> AdminGetAsync(string teamId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(teamId),
            async ct =>
            {
                var team = mapper.MapTo(
                    await teamServiceClient.Admin_GetAsync(
                        new Admin_GetInput { Id = teamId },
                        teamConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: ct));

                var customers = await Task.WhenAll(
                    team.TeamMembers.Select(item => item.Customer.Id).Select(item => customerService.AdminGetAsync(item, ct)));

                foreach (var member in team.TeamMembers)
                {
                    var customer = customers.FirstOrDefault(item => item.Id == member.Customer.Id);
                    if (customer is not null)
                    {
                        member.Customer = customer;
                    }
                }

                if (!string.IsNullOrWhiteSpace(team.PrimaryLocation?.Id))
                {
                    team.PrimaryLocation = await locationService.AdminGetAsync(team.PrimaryLocation.Id, ct);
                }

                return team;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<Team> GetAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(teamId),
            async ct =>
            {
                var team = mapper.MapTo(
                    await teamServiceClient.GetAsync(
                        new GetInput { Id = teamId },
                        teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                        cancellationToken: ct));

                var customers = await Task.WhenAll(
                    team.TeamMembers.Select(item => item.Customer.Id).Select(item => customerService.AdminGetAsync(item, ct)));

                foreach (var member in team.TeamMembers)
                {
                    var customer = customers.FirstOrDefault(item => item.Id == member.Customer.Id);
                    if (customer is not null)
                    {
                        member.Customer = customer;
                    }
                }

                if (!string.IsNullOrWhiteSpace(team.PrimaryLocation?.Id))
                {
                    team.PrimaryLocation = await locationService.GetAsync(workspaceMemberId, team.PrimaryLocation.Id, ct);
                }

                return team;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<(ICollection<Team>, TeamConnection)> GetPaginatedTeamsAsync(
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
            Where = new TeamWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedTeamsInput.OrderBy.Add(new TeamOrderInput { Direction = OrderDirection.Ascending, Field = TeamOrderField.Name });

        var teamsConnection = await teamServiceClient.GetPaginatedTeamsAsync(
            getPaginatedTeamsInput,
            teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var customers = await Task.WhenAll(
            teamsConnection.Edges
                .SelectMany(item => item.Node.Members.Select(member => member.Customer.Id))
                .Select(item => customerService.GetByIdAsync(workspaceMemberId, item, cancellationToken)));

        var teams = teamsConnection.Edges
            .Select(item => mapper.MapTo(item.Node))
            .Select(item =>
            {
                foreach (var member in item.TeamMembers)
                {
                    var matchingCustomer = customers.FirstOrDefault(customer => customer.Id == member.Customer.Id);
                    if (matchingCustomer is not null)
                    {
                        member.Customer = matchingCustomer;
                    }
                }

                return item;
            }).ToList();

        var locations = await Task.WhenAll(
            teams.Where(item => !string.IsNullOrWhiteSpace(item.PrimaryLocation?.Id)).Select(item => item.PrimaryLocation!.Id)
                .Select(item => locationService.GetAsync(workspaceMemberId, item, cancellationToken)));

        foreach (var team in teams)
        {
            if (team.PrimaryLocation is not null)
            {
                var matchingPrimaryLocation = locations.FirstOrDefault(location => location.Id == team.PrimaryLocation.Id);
                if (matchingPrimaryLocation is not null)
                {
                    team.PrimaryLocation = matchingPrimaryLocation;
                }
            }
        }

        await CacheTeamsAsync(teams, cancellationToken);

        return (teams, teamsConnection);
    }

    private async Task CacheTeamsAsync(ICollection<Team> teams, CancellationToken cancellationToken)
    {
        foreach (var team in teams)
        {
            var key = CreateKeyById(team.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                team,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:team-id:{id}";
}
