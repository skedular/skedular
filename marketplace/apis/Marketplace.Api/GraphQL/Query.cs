using System.Reflection;
using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Marketplace.Api.GraphQL;

[QueryType]
public class Query
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
    public async Task<ProductPayload?> ProductAsync(string id, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    [UseResolverScope]
    public async Task<ProductConnection?> ProductsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ProductWhereInput where,
        IEnumerable<ProductOrderInput>? orderBy,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
