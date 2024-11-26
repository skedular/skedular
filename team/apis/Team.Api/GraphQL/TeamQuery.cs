using System.Reflection;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Team.Api.GraphQL;

public class TeamQuery(IMapper mapper)
{
    [UseServiceScope]
    public Version TeamVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    [UseServiceScope]
    public async Task<bool> TeamCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseServiceScope]
    public TeamMemberMembershipType[] TeamMemberMembershipTypes() =>
    [
        TeamMemberMembershipType.Owner, TeamMemberMembershipType.Administrator, TeamMemberMembershipType.Member
    ];

    [UseServiceScope]
    public async Task<TeamDetails?> TeamAsync(
        string id,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(team);
    }

    [UseServiceScope]
    public async Task<TeamConnection?> TeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamWhereInput where,
        TeamOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await teamService.GetPaginatedTeamsAsync(
                new PaginationInputParam(after, first, before, last),
                new TeamSearchCriteria(where.OrganizationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            TeamOrderField.Name => Shared.Models.TeamOrderField.Name,
                            TeamOrderField.About => Shared.Models.TeamOrderField.About,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new TeamOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new TeamConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }

    [UseServiceScope]
    public async Task<TeamDetails[]?> MyTeamsAsync(
        string? organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var teams = await teamService.GetMyTeamsAsync(organizationId, cancellationToken);
        return mapper.MapTo(teams).ToArray();
    }

    [UseServiceScope]
    public async Task<TeamMemberConnection?> TeamMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput where,
        TeamMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.TeamId);

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await teamMemberService.GetPaginatedTeamMembersAsync(
                new PaginationInputParam(after, first, before, last),
                new TeamMemberSearchCriteria(where.TeamId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            TeamMemberOrderField.MembershipType => Shared.Models.TeamMemberOrderField
                                .MembershipType,
                            TeamMemberOrderField.Name => Shared.Models.TeamMemberOrderField.Name,
                            TeamMemberOrderField.GivenName => Shared.Models.TeamMemberOrderField.GivenName,
                            TeamMemberOrderField.MiddleName => Shared.Models.TeamMemberOrderField.MiddleName,
                            TeamMemberOrderField.FamilyName => Shared.Models.TeamMemberOrderField.FamilyName,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new TeamMemberOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new TeamMemberConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }
}
