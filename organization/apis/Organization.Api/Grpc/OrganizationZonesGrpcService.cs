using Api.Shared.Grpc.Skedular.Organization.Zones.V1;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Grpc.Core;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using PageInfo = Api.Shared.Grpc.Skedular.Organization.Core.V1.PageInfo;

namespace Organization.Api.Grpc;

public class OrganizationZonesGrpcService(
    OrganizationConfiguration organizationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITagService tagService,
    IMapper mapper) : OrganizationZonesService.OrganizationZonesServiceBase
{
    public override async Task<ZoneConnection> GetPaginatedZones(GetPaginatedZonesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, [OrganizationTagTypeConstants.Zone], request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ZoneOrderField.Name => OrganizationTagOrderField.Name,
                    ZoneOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new ZoneConnection
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

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseZone));
        return connection;
    }

    public override async Task<Zone> Admin_GetZone(Admin_GetZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<Zone> GetZone(GetZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<Zone> AddZone(AddZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<Zone> UpdateZone(UpdateZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<Zone> RemoveZone(RemoveZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
