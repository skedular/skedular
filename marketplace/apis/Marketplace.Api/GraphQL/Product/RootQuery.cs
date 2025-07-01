using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Shared.Models;

namespace Marketplace.Api.GraphQL.Product;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<ProductDetails?> ProductAsync(string id, [Service] IProductService productService, CancellationToken cancellationToken) =>
        mapper.MapTo(await productService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<ProductEdge>> ProductsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ProductWhereInput where,
        IEnumerable<ProductOrderInput>? orderBy,
        [Service] IProductService productService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await productService.GetPaginatedProductsAsync(
            new PaginationInputParam(after, first, before, last),
            new ProductSearchCriteria(
                where.OrganizationIds.ToSafeCollection(),
                where.ProductIds.ToSafeCollection(),
                where.NameContains,
                where.IncludeInactive),
            orderBy.ToSafeCollection().Select(item => new ProductOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<ProductEdge>
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
