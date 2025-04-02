using System.Reflection;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Team.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version TeamVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> TeamCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public IEnumerable<TeamMemberRole> TeamMemberRoles() => [TeamMemberRole.Owner, TeamMemberRole.Administrator, TeamMemberRole.Member];

    [UseResolverScope]
    public async Task<TeamDetails?> TeamAsync(
        string id,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(team);
    }

    [UseResolverScope]
    public async Task<TeamConnection> TeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamWhereInput where,
        IEnumerable<TeamOrderInput>? orderBy,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamService.GetPaginatedTeamsAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamSearchCriteria(
                where.OrganizationId,
                null,
                where.NameContains,
                where.PrimaryLocationIds),
            orderBy.ToSafeCollection().Select(item => new TeamOrder(item.Direction, item.Field)).ToList(),
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
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<TeamConnection> CustomerTeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomerTeamWhereInput where,
        IEnumerable<TeamOrderInput>? orderBy,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamService.GetPaginatedTeamsAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamSearchCriteria(
                where.OrganizationId,
                where.CustomerId,
                where.NameContains,
                where.PrimaryLocationIds),
            orderBy.ToSafeCollection().Select(item => new TeamOrder(item.Direction, item.Field)).ToList(),
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
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<TeamDetails>> MyTeamsAsync(
        string? organizationId,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await teamService.GetMyTeamsAsync(organizationId, cancellationToken));

    [UseResolverScope]
    public async Task<TeamMemberConnection> TeamMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput where,
        IEnumerable<TeamMemberOrderInput>? orderBy,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.TeamId);

        var (paginatedInfo, edges, totalCount) = await teamMemberService.GetPaginatedMembersAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamMemberSearchCriteria(where.TeamId, where.NameContains),
            orderBy.ToSafeCollection().Select(item => new TeamMemberOrder(item.Direction, item.Field)).ToList(),
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
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }
}
