using System.Reflection;
using Enterprise.Shared.Pagination;
using HotChocolate;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;

namespace Team.Api.GraphQL;

public class TeamQuery
{
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

    public async Task<bool> TeamCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    public TeamMemberMembershipType[] TeamMemberMembershipTypes() =>
    [
        TeamMemberMembershipType.OWNER, TeamMemberMembershipType.ADMINISTRATOR, TeamMemberMembershipType.MEMBER
    ];

    public async Task<TeamDetails?> TeamAsync(
        string id,
        [Service] ITeamService teamService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var team = await teamService.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(team);
    }

    public async Task<TeamConnection?> TeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamWhereInput where,
        TeamOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamService teamService,
        [Service] IMapper mapper,
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
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            TeamOrderField.name =>
                                Shared.Models.TeamOrderField.Name,
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

    public async Task<TeamDetails[]?> MyTeamsAsync(
        string? organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamService teamService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var teams = await teamService.GetMyTeamsAsync(organizationId, cancellationToken);
        return mapper.MapTo(teams).ToArray();
    }

    public async Task<TeamMemberConnection?> PaginatedTeamMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput where,
        TeamMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamMemberService teamMemberService,
        [Service] IMapper mapper,
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
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            TeamMemberOrderField.membershipType => Shared.Models.TeamMemberOrderField
                                .MembershipType,
                            TeamMemberOrderField.name => Shared.Models.TeamMemberOrderField.Name,
                            TeamMemberOrderField.givenName => Shared.Models.TeamMemberOrderField.GivenName,
                            TeamMemberOrderField.middleName => Shared.Models.TeamMemberOrderField.MiddleName,
                            TeamMemberOrderField.familyName => Shared.Models.TeamMemberOrderField.FamilyName,
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

    public async Task<TeamMemberDetails[]?> TeamMembersAsync(
        TeamMemberWhereInput where,
        TeamMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamMemberService teamMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedTeamMembersAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cachedCustomerService,
            teamMemberService,
            mapper,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }
}
