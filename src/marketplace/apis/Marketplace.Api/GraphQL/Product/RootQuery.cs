using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Shared.Models;

namespace Marketplace.Api.GraphQL.Product;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<ProductDetails?> ProductAsync(string id, [Service] IProductService productService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await productService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<ProductDetails?> ProductByIdAsync(
        [ID]
        string id,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken) =>
        await ProductAsync(id, productService, cancellationToken);

    [UseResolverScope]
    public async Task<ProductVersionDetails?> ProductVersionAsync(
        string id,
        [Service]
        IProductVersionService productVersionService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await productVersionService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<ProductVersionDetails?> ProductVersionByIdAsync(
        [ID]
        string id,
        [Service]
        IProductVersionService productVersionService,
        CancellationToken cancellationToken) =>
        await ProductVersionAsync(id, productVersionService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<ProductEdge>> ProductsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ProductWhereInput where,
        IEnumerable<ProductOrderInput>? orderBy,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await productService.GetPaginatedProductsAsync(
            new PaginationInputParam(after, first, before, last),
            new ProductSearchCriteria(
                where.OrganizationIds.ToSafeCollection(),
                where.OrganizationCustomDomains.ToSafeCollection(),
                where.ProductIds.ToSafeCollection(),
                where.IncludeInactive),
            [.. orderBy.ToSafeCollection().Select(item => new ProductOrder(item.Direction, item.Field))],
            cancellationToken);

        return new Connection<ProductEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount,
        };
    }
}
