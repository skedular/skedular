using Customer.Api.GraphQL.Customer;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Settings;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<Connection<CustomerEdge>> CustomersByPreferredLocationAsync(
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
            orderBy.ToSafeCollection().Select(item => new CustomerOrder(item.Direction, item.Field)),
            cancellationToken);

        return new Connection<CustomerEdge>
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
