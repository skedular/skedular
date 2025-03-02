using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Grpc.Core;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Configurations;
using Organization.Shared.Models;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using OrganizationService = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService;
using TermsOfUse = Api.Shared.Services.Grpc.Skedular.Organization.V1.TermsOfUse;
using Version = Api.Shared.Services.Grpc.Skedular.Organization.V1.Version;
using Permissions = Api.Shared.Services.Grpc.Skedular.Organization.V1.Permissions;
using ResourceType = Api.Shared.Services.Grpc.Skedular.Organization.V1.ResourceType;

namespace Organization.Api.Grpc;

public class OrganizationGrpcService(
    OrganizationConfiguration organizationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IOrganizationTermsOfUseService organizationTermsOfUseService,
    IOrganizationService organizationService,
    IOrganizationMemberService organizationMemberService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITagService tagService,
    IResourceTypeService resourceTypeService,
    IMapper mapper) : OrganizationService.OrganizationServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<TermsOfUse> GetActiveOrganizationTermsOfUse(GetActiveOrganizationTermsOfUseInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await organizationTermsOfUseService.GetActiveTermsOfUseAsync(context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await organizationService.AddAsync(
                mapper.MapTo(request),
                request.OfferingCode,
                true,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Get(
        GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var organization = await organizationService.GetByIdAsync(request.Id, context.CancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return mapper.MapToGrpcResponse(organization);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Admin_AddMember(
        Admin_AddMemberInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await organizationMemberService.AddMemberAsync(request.Id, mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<MemberConnection> GetPaginatedMembers(GetPaginatedMembersInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await organizationMemberService.GetPaginatedOrganizationMembersAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new OrganizationMemberSearchCriteria(request.Where.OrganizationId, request.Where.NameContains, request.Where.CustomerId),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    MemberOrderField.RoleType => OrganizationMemberOrderField.Role,
                    MemberOrderField.Status => OrganizationMemberOrderField.Status,
                    MemberOrderField.Name => OrganizationMemberOrderField.Name,
                    MemberOrderField.GivenName => OrganizationMemberOrderField.GivenName,
                    MemberOrderField.MiddleName => OrganizationMemberOrderField.MiddleName,
                    MemberOrderField.FamilyName => OrganizationMemberOrderField.FamilyName,
                    MemberOrderField.PhoneNumber => OrganizationMemberOrderField.PhoneNumber,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new OrganizationMemberOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new MemberConnection
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
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(request.Id, context.CancellationToken);
        return new Permissions
        {
            CanView = permissions.CanView,
            CanModify = permissions.CanModify,
            CanDelete = permissions.CanDelete,
            CanInvitePeople = permissions.CanInvitePeople,
            CanCancelPeopleExistingInvitations = permissions.CanCancelPeopleExistingInvitations,
            CanViewAnalytics = permissions.CanViewAnalytics
        };
    }

    public override async Task<CustomTagConnection> GetPaginatedCustomTags(GetPaginatedCustomTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, OrganizationTagTypeConstants.Custom, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    CustomTagOrderField.CustomTagName => OrganizationTagOrderField.Name,
                    CustomTagOrderField.CustomTagDescription => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new CustomTagConnection
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

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseCustomTag));
        return connection;
    }

    public override async Task<CustomTag> GetCustomTag(GetCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.GetByIdAsync(request.Id, context.CancellationToken));
    }

    public override async Task<CustomTag> AddCustomTag(AddCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<CustomTag> UpdateCustomTag(UpdateCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<CustomTag> RemoveCustomTag(RemoveCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<ZoneConnection> GetPaginatedZones(GetPaginatedZonesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, OrganizationTagTypeConstants.Zone, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ZoneOrderField.ZoneName => OrganizationTagOrderField.Name,
                    ZoneOrderField.ZoneDescription => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
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

    public override async Task<Zone> GetZone(GetZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.GetByIdAsync(request.Id, context.CancellationToken));
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

    public override async Task<ResourceTypeConnection> GetPaginatedResourceTypes(GetPaginatedResourceTypesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await resourceTypeService.GetPaginatedResourceTypesAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new ResourceTypeSearchCriteria(request.Where.OrganizationId, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ResourceTypeOrderField.ResourceTypeName => OrganizationResourceTypeOrderField.Name,
                    ResourceTypeOrderField.ResourceTypeDescription => OrganizationResourceTypeOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new ResourceTypeOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new ResourceTypeConnection
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

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseResourceType));
        return connection;
    }

    public override async Task<ResourceType> GetResourceType(GetResourceTypeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseResourceType(await resourceTypeService.GetByIdAsync(request.Id, context.CancellationToken));
    }

    public override async Task<ResourceType> AddResourceType(AddResourceTypeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseResourceType(await resourceTypeService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<ResourceType> UpdateResourceType(UpdateResourceTypeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseResourceType(await resourceTypeService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<ResourceType> RemoveResourceType(RemoveResourceTypeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseResourceType(await resourceTypeService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
