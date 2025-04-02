using System.Reflection;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Marketplace.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version MarketplaceVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> MarketplaceCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public IEnumerable<CurrencyDetails> Currencies() =>
    [
        new() { Type = Currency.Nzd, Name = Currency.Nzd.ToCurrencyName() },
        new() { Type = Currency.Usd, Name = Currency.Usd.ToCurrencyName() }
    ];

    [UseResolverScope]
    public IEnumerable<PriceUnitDetails> PriceUnits() =>
    [
        new() { Type = PriceUnit.PerMinute, Name = PriceUnit.PerMinute.ToPriceUnitName() },
        new() { Type = PriceUnit.PerHour, Name = PriceUnit.PerHour.ToPriceUnitName() },
        new() { Type = PriceUnit.PerUse, Name = PriceUnit.PerUse.ToPriceUnitName() }
    ];

    [UseResolverScope]
    public async Task<ProductDetails?> ProductAsync(string id, [Service] IProductService productService, CancellationToken cancellationToken) =>
        mapper.MapTo(await productService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<ProductConnection?> ProductsAsync(
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

        return new ProductConnection
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
