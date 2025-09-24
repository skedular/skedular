using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;
using Resource = Slack.Shared.Models.Resource;
using ResourceEdge = Slack.Shared.Models.ResourceEdge;
using ResourceType = Slack.Shared.Models.ResourceType;

namespace Slack.Shared.Services.CrossDomains;

public interface ILocationResourceService
{
    Task<Resource> AdminGetAsync(string resourceId, CancellationToken cancellationToken);
    Task<Resource> AddAsync(string workspaceMemberId, Resource resource, CancellationToken cancellationToken);
    Task<Resource> UpdateAsync(string workspaceMemberId, Resource resource, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken);
    Task<Resource> GetAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken);

    Task<Connection<ResourceEdge>> GetPaginatedResourcesAsync(
        string workspaceMemberId,
        string locationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class LocationResourceService(
    ApplicationConfiguration applicationConfiguration,
    LocationConfiguration locationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient,
    IMapper mapper,
    HybridCache hybridCache,
    IOrganizationCustomTagService organizationCustomTagService,
    IOrganizationZoneService organizationZoneService,
    IOrganizationProductTagService organizationProductTagService,
    IOrganizationTagService organizationTagService) : ILocationResourceService
{
    public async Task<Resource> AdminGetAsync(string resourceId, CancellationToken cancellationToken) =>
        await AdminEnrichAsync(
            await hybridCache.GetOrCreateAsync(
                CreateKeyById(resourceId),
                async ct => mapper.MapTo(
                    await locationServiceClient.Admin_GetResourceAsync(
                        new Admin_GetResourceInput { Id = resourceId },
                        locationConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: ct)),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken),
            cancellationToken);

    public async Task<Resource> AddAsync(string workspaceMemberId, Resource resource, CancellationToken cancellationToken)
    {
        var addResourceInput = new AddResourceInput
        {
            Id = resource.Id,
            Name = resource.Name.ToSafeString(),
            Capacity = resource.Capacity,
            Color = resource.Color.ToSafeString(),
            Inactive = resource.Inactive,
            RequireBookingApproval = resource.RequireBookingApproval,
            LocationId = resource.Location!.Id
        };

        addResourceInput.TagIds.AddRange(resource.OrganizationCustomTags.Select(item => item.Id));
        addResourceInput.TagIds.AddRange(resource.OrganizationZones.Select(item => item.Id));
        addResourceInput.TagIds.AddRange(resource.OrganizationProductTags.Select(item => item.Id));
        addResourceInput.TagIds.Add(resource.ResourceType.Id);

        var mappedResource = mapper.MapTo(
            await locationServiceClient.AddResourceAsync(
                addResourceInput,
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedResource], cancellationToken);

        return await EnrichAsync(workspaceMemberId, mappedResource, cancellationToken);
    }

    public async Task<Resource> UpdateAsync(string workspaceMemberId, Resource resource, CancellationToken cancellationToken)
    {
        var updateZoneInput = new UpdateResourceInput
        {
            Id = resource.Id,
            Name = resource.Name.ToSafeString(),
            Capacity = resource.Capacity,
            Color = resource.Color.ToSafeString(),
            Inactive = resource.Inactive,
            RequireBookingApproval = resource.RequireBookingApproval
        };

        updateZoneInput.TagIds.AddRange(resource.OrganizationCustomTags.Select(item => item.Id));
        updateZoneInput.TagIds.AddRange(resource.OrganizationZones.Select(item => item.Id));
        updateZoneInput.TagIds.AddRange(resource.OrganizationProductTags.Select(item => item.Id));
        updateZoneInput.TagIds.Add(resource.ResourceType.Id);

        var mappedResource = mapper.MapTo(
            await locationServiceClient.UpdateResourceAsync(
                updateZoneInput,
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedResource], cancellationToken);

        return await EnrichAsync(workspaceMemberId, mappedResource, cancellationToken);
    }

    public async Task RemoveAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken)
    {
        await locationServiceClient.RemoveResourceAsync(
            new RemoveResourceInput { Id = resourceId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(resourceId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<Resource> GetAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken) =>
        await EnrichAsync(
            workspaceMemberId,
            await hybridCache.GetOrCreateAsync(
                CreateKeyById(resourceId),
                async ct => mapper.MapTo(
                    await locationServiceClient.GetResourceAsync(
                        new GetResourceInput { Id = resourceId },
                        locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                        cancellationToken: ct)),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken),
            cancellationToken);

    public async Task<Connection<ResourceEdge>> GetPaginatedResourcesAsync(
        string workspaceMemberId,
        string locationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedResourcesInput = new GetPaginatedResourcesInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new ResourceWhereInput { LocationId = locationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedResourcesInput.OrderBy.Add(new ResourceOrderInput
        {
            Direction = OrderDirection.Ascending, Field = ResourceOrderField.ResourceName
        });

        var connection = await locationServiceClient.GetPaginatedResourcesAsync(
            getPaginatedResourcesInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);
        var edges = connection.Edges.Select(item => new ResourceEdge(mapper.MapTo(item.Node), item.Cursor)).ToList();

        await CacheAsync(edges.Select(item => item.Node).ToList(), cancellationToken);

        var enrichedEdges = new List<ResourceEdge>();
        foreach (var item in edges)
        {
            enrichedEdges.Add(new ResourceEdge(await EnrichAsync(workspaceMemberId, item.Node, cancellationToken), item.Cursor));
        }

        return new Connection<ResourceEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = enrichedEdges
        };
    }

    private async Task CacheAsync(ICollection<Resource> resources, CancellationToken cancellationToken)
    {
        foreach (var resource in resources)
        {
            var key = CreateKeyById(resource.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                resource,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private async Task<Resource> AdminEnrichAsync(Resource resource, CancellationToken cancellationToken)
    {
        var customTags = await Task.WhenAll(
            resource.OrganizationCustomTags
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationCustomTagService.AdminGetAsync(item, cancellationToken)));

        var zones = await Task.WhenAll(
            resource.OrganizationZones
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationZoneService.AdminGetAsync(item, cancellationToken)));

        var productTags = await Task.WhenAll(
            resource.OrganizationProductTags
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationProductTagService.AdminGetAsync(item, cancellationToken)));

        var resourceType = await organizationTagService.AdminGetAsync(resource.ResourceType.Id, cancellationToken);
        resource.ResourceType = new ResourceType
        {
            Id = resourceType.Id,
            Name = resourceType.Name.ToSafeString(),
            Description = resourceType.Description.ToSafeString(),
            Color = resourceType.Color.ToSafeString(),
            Type = resourceType.Type
        };

        resource.OrganizationCustomTags = resource.OrganizationCustomTags
            .Select(item => customTags.FirstOrDefault(organizationCustomTag => organizationCustomTag.Id == item.Id) ?? item)
            .ToList();

        resource.OrganizationZones = resource.OrganizationZones
            .Select(item => zones.FirstOrDefault(organizationZone => organizationZone.Id == item.Id) ?? item)
            .ToList();

        resource.OrganizationProductTags = resource.OrganizationProductTags
            .Select(item => productTags.FirstOrDefault(organizationProductTag => organizationProductTag.Id == item.Id) ?? item)
            .ToList();

        return resource;
    }

    private async Task<Resource> EnrichAsync(string workspaceMemberId, Resource resource, CancellationToken cancellationToken)
    {
        var customTags = await Task.WhenAll(
            resource.OrganizationCustomTags
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationCustomTagService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var zones = await Task.WhenAll(
            resource.OrganizationZones
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationZoneService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var productTags = await Task.WhenAll(
            resource.OrganizationProductTags
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationProductTagService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var resourceType = await organizationTagService.GetAsync(workspaceMemberId, resource.ResourceType.Id, cancellationToken);
        resource.ResourceType = new ResourceType
        {
            Id = resourceType.Id,
            Name = resourceType.Name.ToSafeString(),
            Description = resourceType.Description.ToSafeString(),
            Color = resourceType.Color.ToSafeString(),
            Type = resourceType.Type
        };

        resource.OrganizationCustomTags = resource.OrganizationCustomTags
            .Select(item => customTags.FirstOrDefault(organizationCustomTag => organizationCustomTag.Id == item.Id) ?? item)
            .ToList();

        resource.OrganizationZones = resource.OrganizationZones
            .Select(item => zones.FirstOrDefault(organizationZone => organizationZone.Id == item.Id) ?? item)
            .ToList();

        resource.OrganizationProductTags = resource.OrganizationProductTags
            .Select(item => productTags.FirstOrDefault(organizationProductTag => organizationProductTag.Id == item.Id) ?? item)
            .ToList();

        return resource;
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:location-resource-id:{id}";
}
