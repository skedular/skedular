using System.Reflection;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;

namespace Team.Api.GraphQL;

public class TeamQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> TeamVersionAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public async Task<bool> TeamCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public Task<TeamMemberMembershipType[]> TeamMemberMembershipTypesAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new[]
        {
            TeamMemberMembershipType.OWNER, TeamMemberMembershipType.ADMINISTRATOR, TeamMemberMembershipType.MEMBER
        });

    public async Task<TeamDetails?> TeamAsync(string id, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var team = await service.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(team);
    }

    public async Task<TeamConnection?> TeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamWhereInput where,
        TeamOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedTeamsAsync(
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

    public async Task<TeamDetails[]?> MyTeamsAsync(string? organizationId, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var teams = await service.GetMyTeamsAsync(organizationId, cancellationToken);
        return mapper.MapTo(teams).ToArray();
    }

    public async Task<TeamMemberConnection?> PaginatedTeamMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput where,
        TeamMemberOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.TeamId);

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<ITeamMemberService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedTeamMembersAsync(
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
        CancellationToken cancellationToken)
    {
        var result = await PaginatedTeamMembersAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }
}
