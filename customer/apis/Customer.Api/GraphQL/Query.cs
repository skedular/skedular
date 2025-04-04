using System.Reflection;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Customer.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version CustomerVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<CustomerDetails?> MeAsync([Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetMeAsync(true, cancellationToken));

    [UseResolverScope]
    public async Task<CustomerDetails?> CustomerAsync(string id, [Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    public async Task<CustomerConnection> CustomersByPreferredLocationAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomersByPreferredLocationWhereInput where,
        IEnumerable<CustomerOrderInput>? orderBy,
        [Service] ICustomerService customerService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.LocationId);

        var (paginatedInfo, edges, totalCount) = await customerService.GetPaginatedCustomersAsync(
            new PaginationInputParam(after, first, before, last),
            new CustomerSearchCriteria(where.NameContains, where.LocationId),
            orderBy.ToSafeCollection().Select(item => new CustomerOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new CustomerConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }
}
