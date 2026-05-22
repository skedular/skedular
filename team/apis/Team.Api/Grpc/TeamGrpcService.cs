using Api.Shared.Grpc.Skedular.Team.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using Team.Api.Mappers;
using Team.Api.Models;
using Team.Api.Services;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using OrderDirection = Api.Shared.Grpc.Skedular.Team.Core.V1.OrderDirection;
using Permissions = Api.Shared.Grpc.Skedular.Team.Core.V1.Permissions;
using TeamOrderField = Api.Shared.Grpc.Skedular.Team.Core.V1.TeamOrderField;
using TeamPatchField = Api.Shared.Grpc.Skedular.Team.Core.V1.TeamPatchField;
using TeamService = Api.Shared.Grpc.Skedular.Team.Core.V1.TeamService;
using Version = Api.Shared.Grpc.Skedular.Team.Core.V1.Version;

namespace Team.Api.Grpc;

public class TeamGrpcService(
    IVersionService versionService,
    TeamConfiguration teamConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITeamService teamService,
    ITeamAuthorizationService teamAuthorizationService,
    IGrpcMapper grpcMapper,
    ILogger<TeamGrpcService> logger) : TeamService.TeamServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team> Admin_Get(Admin_GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var team = await teamService.GetByIdAsync(request.Id, true, context.CancellationToken) ?? throw new TeamNotFound();
        logger.LogInformation("gRPC Admin_Get resolved team {TeamId}", request.Id);

        return grpcMapper.MapToGrpcResponse(team);
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var team = await teamService.GetByIdAsync(request.Id, false, context.CancellationToken) ?? throw new TeamNotFound();
        logger.LogInformation("gRPC Get resolved team {TeamId}", request.Id);

        return grpcMapper.MapToGrpcResponse(team);
    }

    public override async Task<TeamConnection> GetPaginatedTeams(GetPaginatedTeamsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await teamService.GetPaginatedTeamsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TeamSearchCriteria(
                request.Where.OrganizationId,
                null,
                request.Where.CustomerId,
                request.Where.NameContains,
                request.Where.PrimaryLocationIds.ToSafeCollection()),
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

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));

        if (totalCount == 0)
        {
            logger.LogInformation("gRPC GetPaginatedTeams returned no teams for organization {OrganizationId}", request.Where.OrganizationId);
        }

        return connection;
    }

    public override async Task<Permissions> GetPermissions(GetPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var permissions = await teamAuthorizationService.GetPermissionsAsync(request.Id, context.CancellationToken);
        logger.LogInformation("gRPC GetPermissions resolved for team {TeamId}", request.Id);
        return new Permissions
        {
            CanView = permissions.CanView,
            CanModify = permissions.CanModify,
            CanDelete = permissions.CanDelete,
            CanInvitePeople = permissions.CanInvitePeople,
            CanCancelPeopleExistingInvitations = permissions.CanCancelPeopleExistingInvitations
        };
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team> Add(AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrEmpty(request.OrganizationId);

        var added = await teamService.AddAsync(grpcMapper.MapTo(request), context.CancellationToken);
        logger.LogInformation("gRPC Add created team {TeamId}", added.Id);

        return grpcMapper.MapToGrpcResponse(added);
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team> Update(UpdateInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrEmpty(request.OrganizationId);

        var requestedTeam = grpcMapper.MapTo(request);
        var fields = request.FieldsToUpdate.ToHashSet();
        var patchFields = new HashSet<TeamAndMembersPatchField>();
        if (fields.Any(field => field != TeamPatchField.Members))
        {
            patchFields.Add(TeamAndMembersPatchField.Team);
        }

        if (fields.Contains(TeamPatchField.Members))
        {
            patchFields.Add(TeamAndMembersPatchField.Members);
        }

        var updated = await teamService.UpdateAsync(
            new TeamAndMembersPatchRequest(requestedTeam, patchFields),
            context.CancellationToken);
        logger.LogInformation("gRPC Update modified team {TeamId}", updated.Id);

        return grpcMapper.MapToGrpcResponse(updated);
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team> Remove(
        RemoveInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var deleted = await teamService.DeleteAsync(request.Id, context.CancellationToken);
        logger.LogInformation("gRPC Remove deleted team {TeamId}", request.Id);

        return grpcMapper.MapToGrpcResponse(deleted);
    }
}
