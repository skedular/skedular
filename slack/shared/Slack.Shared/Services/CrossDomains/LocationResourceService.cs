using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Api.Shared.Grpc.Skedular.Location.Resources.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
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
    LocationResourcesService.LocationResourcesServiceClient locationResourcesServiceClient,
    IMapper mapper,
    IMemoryCache memoryCache,
    IOrganizationCustomTagService organizationCustomTagService,
    IOrganizationZoneService organizationZoneService,
    IOrganizationProductTagService organizationProductTagService,
    IOrganizationTagService organizationTagService) : ILocationResourceService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<Resource> AdminGetAsync(string resourceId, CancellationToken cancellationToken) =>
        await AdminEnrichAsync(
            (await memoryCache.GetOrCreateAsync(
                CreateKeyById(resourceId),
                async _ => mapper.MapTo(
                    await locationResourcesServiceClient.Admin_GetResourceAsync(
                        new Admin_GetResourceInput { Id = resourceId },
                        locationConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken)),
                _cacheEntryOptions))!,
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

        addResourceInput.TagIds.AddRange(resource.CustomTags.Select(item => item.Id));
        addResourceInput.TagIds.AddRange(resource.Zones.Select(item => item.Id));
        addResourceInput.TagIds.AddRange(resource.ProductTags.Select(item => item.Id));
        addResourceInput.TagIds.Add(resource.ResourceType.Id);

        var mappedResource = mapper.MapTo(
            await locationResourcesServiceClient.AddResourceAsync(
                addResourceInput,
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedResource]);

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

        updateZoneInput.TagIds.AddRange(resource.CustomTags.Select(item => item.Id));
        updateZoneInput.TagIds.AddRange(resource.Zones.Select(item => item.Id));
        updateZoneInput.TagIds.AddRange(resource.ProductTags.Select(item => item.Id));
        updateZoneInput.TagIds.Add(resource.ResourceType.Id);

        var mappedResource = mapper.MapTo(
            await locationResourcesServiceClient.UpdateResourceAsync(
                updateZoneInput,
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedResource]);

        return await EnrichAsync(workspaceMemberId, mappedResource, cancellationToken);
    }

    public async Task RemoveAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken)
    {
        await locationResourcesServiceClient.RemoveResourceAsync(
            new RemoveResourceInput { Id = resourceId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(resourceId);

        memoryCache.Remove(key);
    }

    public async Task<Resource> GetAsync(string workspaceMemberId, string resourceId, CancellationToken cancellationToken) =>
        await EnrichAsync(
            workspaceMemberId,
            (await memoryCache.GetOrCreateAsync(
                CreateKeyById(resourceId),
                async _ => mapper.MapTo(
                    await locationResourcesServiceClient.GetResourceAsync(
                        new GetResourceInput { Id = resourceId },
                        locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                        cancellationToken: cancellationToken)),
                _cacheEntryOptions))!,
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

        var connection = await locationResourcesServiceClient.GetPaginatedResourcesAsync(
            getPaginatedResourcesInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);
        var edges = connection.Edges.Select(item => new ResourceEdge(mapper.MapTo(item.Node), item.Cursor)).ToList();

        Cache(edges.Select(item => item.Node).ToList());

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

    private void Cache(IReadOnlyList<Resource> resources)
    {
        foreach (var resource in resources)
        {
            var key = CreateKeyById(resource.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, resource, _cacheEntryOptions);
        }
    }

    private async Task<Resource> AdminEnrichAsync(Resource resource, CancellationToken cancellationToken)
    {
        var customTags = await Task.WhenAll(
            resource.CustomTags
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationCustomTagService.AdminGetAsync(item, cancellationToken)));

        var zones = await Task.WhenAll(
            resource.Zones
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationZoneService.AdminGetAsync(item, cancellationToken)));

        var productTags = await Task.WhenAll(
            resource.ProductTags
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

        resource.CustomTags = resource.CustomTags
            .Select(item => customTags.FirstOrDefault(organizationCustomTag => organizationCustomTag.Id == item.Id) ?? item)
            .ToList();

        resource.Zones = resource.Zones
            .Select(item => zones.FirstOrDefault(organizationZone => organizationZone.Id == item.Id) ?? item)
            .ToList();

        resource.ProductTags = resource.ProductTags
            .Select(item => productTags.FirstOrDefault(organizationProductTag => organizationProductTag.Id == item.Id) ?? item)
            .ToList();

        return resource;
    }

    private async Task<Resource> EnrichAsync(string workspaceMemberId, Resource resource, CancellationToken cancellationToken)
    {
        var customTags = await Task.WhenAll(
            resource.CustomTags
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationCustomTagService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var zones = await Task.WhenAll(
            resource.Zones
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationZoneService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var productTags = await Task.WhenAll(
            resource.ProductTags
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

        resource.CustomTags = resource.CustomTags
            .Select(item => customTags.FirstOrDefault(organizationCustomTag => organizationCustomTag.Id == item.Id) ?? item)
            .ToList();

        resource.Zones = resource.Zones
            .Select(item => zones.FirstOrDefault(organizationZone => organizationZone.Id == item.Id) ?? item)
            .ToList();

        resource.ProductTags = resource.ProductTags
            .Select(item => productTags.FirstOrDefault(organizationProductTag => organizationProductTag.Id == item.Id) ?? item)
            .ToList();

        return resource;
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:location-resource-id:{id}";
}
