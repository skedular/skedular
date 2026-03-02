using Api.Shared.Services.Models;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.GraphQL.Product;
using Marketplace.Shared.Services.Cache;
using Version = Enterprise.Shared.GraphQL.Types.Version;
using Constants = Api.Shared.Services.Constants;

namespace Marketplace.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    public int DefaultMaxAllowedResourcesLockTimePaidViaCard => Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard;
    public int DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer => Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer;

    public ICollection<ProductPricingCadenceDetails> ProductPricingCadences =>
    [
        new() { Type = ProductPricingCadence.OneTimeV1, Name = ProductPricingCadence.OneTimeV1.ToProductPricingCadenceName() },
        new()
        {
            Type = ProductPricingCadence.PerMinuteV1, Name = ProductPricingCadence.PerMinuteV1.ToProductPricingCadenceName()
        },
        new() { Type = ProductPricingCadence.PerHourV1, Name = ProductPricingCadence.PerHourV1.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.DailyV1, Name = ProductPricingCadence.DailyV1.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.WeeklyV1, Name = ProductPricingCadence.WeeklyV1.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.MonthlyV1, Name = ProductPricingCadence.MonthlyV1.ToProductPricingCadenceName() }
    ];

    public ICollection<PaymentMethodTypeDetails> PaymentMethods =>
    [
        new() { Type = PaymentMethod.Card, Name = PaymentMethod.Card.ToPaymentMethodName() },
        new() { Type = PaymentMethod.BankTransfer, Name = PaymentMethod.BankTransfer.ToPaymentMethodName() }
    ];

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
}
