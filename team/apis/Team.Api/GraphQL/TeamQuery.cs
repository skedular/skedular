using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Team;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;
using OrderDirection = Api.Shared.Services.GraphQL.UnityHub.V1.Team.OrderDirection;
using PageInfo = Api.Shared.Services.GraphQL.UnityHub.V1.Team.PageInfo;
using Query = Api.Shared.Services.GraphQL.UnityHub.V1.Team.Query;
using TeamMemberOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Team.TeamMemberOrderInput;
using TeamOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Team.TeamOrderInput;
using TeamOrderField = Api.Shared.Services.GraphQL.UnityHub.V1.Team.TeamOrderField;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Team.Version;
using TeamMemberOrderField = Api.Shared.Services.GraphQL.UnityHub.V1.Team.TeamMemberOrderField;

namespace Team.Api.GraphQL;

public class TeamQuery(IMapper mapper) : Query
{
    public override Task<Version> TeamVersionAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
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

    public override async Task<bool> TeamCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override Task<TeamMemberMembershipType[]> TeamMemberMembershipTypesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken) => Task.FromResult(new[]
    {
        TeamMemberMembershipType.OWNER, TeamMemberMembershipType.ADMINISTRATOR, TeamMemberMembershipType.MEMBER
    });

    public override async Task<TeamDetails?> TeamAsync(
        string id,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var team = await service.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(team);
    }

    public override async Task<TeamConnection> TeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamWhereInput where,
        TeamOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new TeamConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
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

    public override async Task<TeamDetails[]> MyTeamsAsync(
        string? organizationId,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return [];
        }

        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var teams = await service.GetMyTeamsAsync(organizationId, cancellationToken);
        return mapper.MapTo(teams).ToArray();
    }

    public override async Task<TeamMemberConnection> PaginatedTeamMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput where,
        TeamMemberOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(where.TeamId))
        {
            return new TeamMemberConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new TeamMemberConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
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

    public override async Task<TeamMemberDetails[]> TeamMembersAsync(
        TeamMemberWhereInput where,
        TeamMemberOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedTeamMembersAsync(null, null, null, null, where, [], serviceProvider,
            cancellationToken);
        return result.Edges.Select(item => item.Node).ToArray();
    }
}
