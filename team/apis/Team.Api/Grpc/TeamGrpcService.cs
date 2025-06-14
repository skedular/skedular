using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Api.Services.Authorization;
using Team.Shared.Configurations;
using Team.Shared.Models;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Team.V1.OrderDirection;
using Permissions = Api.Shared.Services.Grpc.Skedular.Team.V1.Permissions;
using TeamOrderField = Api.Shared.Services.Grpc.Skedular.Team.V1.TeamOrderField;
using TeamService = Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService;
using Version = Api.Shared.Services.Grpc.Skedular.Team.V1.Version;

namespace Team.Api.Grpc;

public class TeamGrpcService(
    IVersionService versionService,
    TeamConfiguration teamConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITeamService teamService,
    ITeamAuthorizationService teamAuthorizationService,
    IMapper mapper) : TeamService.TeamServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Admin_Get(Admin_GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var team = await teamService.GetByIdAsync(request.Id, true, context.CancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        return mapper.MapToGrpcResponse(team);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var team = await teamService.GetByIdAsync(request.Id, false, context.CancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        return mapper.MapToGrpcResponse(team);
    }

    public override async Task<TeamConnection> GetPaginatedTeams(GetPaginatedTeamsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await teamService.GetPaginatedTeamsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TeamSearchCriteria(
                request.Where.OrganizationId,
                request.Where.CustomerId,
                request.Where.NameContains,
                request.Where.PrimaryLocationIds),
            request.OrderBy.Select(item =>
            {
                var field = item.Field switch
                {
                    TeamOrderField.Name => Shared.Models.TeamOrderField.Name,
                    TeamOrderField.About => Shared.Models.TeamOrderField.About,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var direction = item.Direction == OrderDirection.Ascending
                    ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                    : Enterprise.Shared.Pagination.OrderDirection.Descending;
                return new TeamOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new TeamConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));

        return connection;
    }

    public override async Task<Permissions> GetPermissions(GetPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var permissions = await teamAuthorizationService.GetPermissionsAsync(request.Id, context.CancellationToken);
        return new Permissions
        {
            CanView = permissions.CanView,
            CanModify = permissions.CanModify,
            CanDelete = permissions.CanDelete,
            CanInvitePeople = permissions.CanInvitePeople,
            CanCancelPeopleExistingInvitations = permissions.CanCancelPeopleExistingInvitations
        };
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Add(AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrEmpty(request.OrganizationId);

        return mapper.MapToGrpcResponse(await teamService.AddAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Update(UpdateInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrEmpty(request.OrganizationId);

        return mapper.MapToGrpcResponse(await teamService.UpdateAsync(mapper.MapTo(request), true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Remove(
        RemoveInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await teamService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
