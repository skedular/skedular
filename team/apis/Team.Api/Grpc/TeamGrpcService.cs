using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using HotChocolate.Subscriptions;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Api.Services.Authorization;
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
    IMapper mapper,
    ITopicEventSender topicEventSender,
    ILogger<TeamGrpcService> logger) : TeamService.TeamServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);
        logger.LogInformation("gRPC RaiseGraphqlChange sent for topic {TopicName} and id {EntityId}", request.TopicName, request.Id);

        return new RaiseGraphqlChangeResponse();
    }

    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Admin_Get(Admin_GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var team = await teamService.GetByIdAsync(request.Id, true, context.CancellationToken) ?? throw new TeamNotFound();
        logger.LogInformation("gRPC Admin_Get resolved team {TeamId}", request.Id);

        return mapper.MapToGrpcResponse(team);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var team = await teamService.GetByIdAsync(request.Id, false, context.CancellationToken) ?? throw new TeamNotFound();
        logger.LogInformation("gRPC Get resolved team {TeamId}", request.Id);

        return mapper.MapToGrpcResponse(team);
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

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));

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

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Add(AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrEmpty(request.OrganizationId);

        var added = await teamService.AddAsync(mapper.MapTo(request), context.CancellationToken);
        logger.LogInformation("gRPC Add created team {TeamId}", added.Id);

        return mapper.MapToGrpcResponse(added);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Update(UpdateInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrEmpty(request.OrganizationId);

        var updated = await teamService.UpdateAsync(mapper.MapTo(request), true, context.CancellationToken);
        logger.LogInformation("gRPC Update modified team {TeamId}", updated.Id);

        return mapper.MapToGrpcResponse(updated);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team> Remove(
        RemoveInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        var deleted = await teamService.DeleteAsync(request.Id, context.CancellationToken);
        logger.LogInformation("gRPC Remove deleted team {TeamId}", request.Id);

        return mapper.MapToGrpcResponse(deleted);
    }
}
