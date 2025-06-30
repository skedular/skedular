using Api.Shared.Services.Models;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.GraphQL.Product;
using Marketplace.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Marketplace.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    [UseResolverScope]
    public Version MarketplaceVersion()
    {
        var version = versionService.GetVersion();

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
}
