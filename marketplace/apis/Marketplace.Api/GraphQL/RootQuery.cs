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

    public IReadOnlyList<ProductPricingCadenceDetails> ProductPricingCadences =>
    [
        new() { Type = ProductPricingCadence.OneTime, Name = ProductPricingCadence.OneTime.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.PerMinute, Name = ProductPricingCadence.PerMinute.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Per15Minutes, Name = ProductPricingCadence.Per15Minutes.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Per30Minutes, Name = ProductPricingCadence.Per30Minutes.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.PerHour, Name = ProductPricingCadence.PerHour.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.HalfDay, Name = ProductPricingCadence.HalfDay.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Daily, Name = ProductPricingCadence.Daily.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Weekly, Name = ProductPricingCadence.Weekly.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Fortnightly, Name = ProductPricingCadence.Fortnightly.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Monthly, Name = ProductPricingCadence.Monthly.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.TwoMonths, Name = ProductPricingCadence.TwoMonths.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Quarterly, Name = ProductPricingCadence.Quarterly.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.FourMonths, Name = ProductPricingCadence.FourMonths.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.FiveMonths, Name = ProductPricingCadence.FiveMonths.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.SixMonths, Name = ProductPricingCadence.SixMonths.ToProductPricingCadenceName() },
        new() { Type = ProductPricingCadence.Yearly, Name = ProductPricingCadence.Yearly.ToProductPricingCadenceName() }
    ];

    public IReadOnlyList<ProductPricingBillingModeDetails> ProductPricingBillingModes =>
    [
        new() { Type = ProductPricingBillingMode.Upfront, Name = ProductPricingBillingMode.Upfront.ToProductPricingBillingModeName() },
        new() { Type = ProductPricingBillingMode.InArrears, Name = ProductPricingBillingMode.InArrears.ToProductPricingBillingModeName() }
    ];

    public IReadOnlyList<PaymentMethodTypeDetails> PaymentMethods =>
    [
        new() { Type = PaymentMethod.Card, Name = PaymentMethod.Card.ToPaymentMethodName() },
        new() { Type = PaymentMethod.BankTransfer, Name = PaymentMethod.BankTransfer.ToPaymentMethodName() }
    ];

    public IReadOnlyList<ProductPricingCancellationTypeDetails> ProductPricingCancellationTypes =>
    [
        new()
        {
            Type = ProductPricingCancellationPolicyType.NoCancellation,
            Name = ProductPricingCancellationPolicyType.NoCancellation.ToProductPricingCancellationPolicyTypeName()
        },
        new()
        {
            Type = ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            Name = ProductPricingCancellationPolicyType.FullRefundBeforeCutoff.ToProductPricingCancellationPolicyTypeName()
        },
        new()
        {
            Type = ProductPricingCancellationPolicyType.TieredRefund,
            Name = ProductPricingCancellationPolicyType.TieredRefund.ToProductPricingCancellationPolicyTypeName()
        }
    ];

    public IReadOnlyList<ProductTypeDetails> ProductTypes =>
    [
        new() { Type = ProductType.Resource, Name = ProductType.Resource.ToProductTypeName() },
        new() { Type = ProductType.Event, Name = ProductType.Event.ToProductTypeName() }
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
