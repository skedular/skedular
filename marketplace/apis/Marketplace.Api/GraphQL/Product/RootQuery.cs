using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Shared.Models;
using Constants = Api.Shared.Services.Constants;

namespace Marketplace.Api.GraphQL.Product;

[QueryType]
public class RootQuery(IMapper mapper)
{
    public int DefaultMaxAllowedResourcesLockTimePaidViaCard => Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard;
    public int DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer => Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer;

    [UseResolverScope]
    public async Task<ProductDetails?> ProductAsync(string id, [Service] IProductService productService, CancellationToken cancellationToken) =>
        mapper.MapTo(await productService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<ProductDetails?> ProductByIdAsync(
        string id,
        [Service] IProductService productService,
        CancellationToken cancellationToken) =>
        await ProductAsync(id, productService, cancellationToken);

    [UseResolverScope]
    public async Task<ProductVersionDetails?> ProductVersionAsync(string id, [Service] IProductVersionService productVersionService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await productVersionService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<ProductVersionDetails?> ProductVersionByIdAsync(
        string id,
        [Service] IProductVersionService productVersionService,
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
        [Service] IProductService productService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await productService.GetPaginatedProductsAsync(
            new PaginationInputParam(after, first, before, last),
            new ProductSearchCriteria(
                where.OrganizationUniqueAlphanumericNames.ToSafeCollection(),
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
