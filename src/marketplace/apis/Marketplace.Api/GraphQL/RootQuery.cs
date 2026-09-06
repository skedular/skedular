using Api.Shared.Services.Models;
using Enterprise.Shared.Version;
using HotChocolate.Types;
using Marketplace.Api.GraphQL.Product;
using Version = Enterprise.Shared.GraphQL.Types.Version;
using Constants = Api.Shared.Services.Constants;

namespace Marketplace.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    public int DefaultMaxAllowedResourcesLockTimePaidViaCard => Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard;
    public int DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer => Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer;

    public IEnumerable<DurationDisplayUnitDetails> DurationDisplayUnits =>
    [
        new()
        {
            Type = DurationDisplayUnit.Minutes,
            Name = DurationDisplayUnit.Minutes.ToDurationDisplayUnitName(),
        },
        new()
        {
            Type = DurationDisplayUnit.Hours,
            Name = DurationDisplayUnit.Hours.ToDurationDisplayUnitName(),
        },
    ];

    public IEnumerable<ProductPricingFulfillmentTypeDetails> ProductPricingFulfillmentTypes =>
    [
        new()
        {
            Type = ProductPricingFulfillmentType.Reservation,
            Name = ProductPricingFulfillmentType.Reservation.ToProductPricingFulfillmentTypeName(),
        },
        new()
        {
            Type = ProductPricingFulfillmentType.Entitlement,
            Name = ProductPricingFulfillmentType.Entitlement.ToProductPricingFulfillmentTypeName(),
        },
    ];

    public IEnumerable<MembershipTermDetails> MembershipTerms =>
    [
        new()
        {
            Type = MembershipTerm.Daily,
            Name = MembershipTerm.Daily.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.Weekly,
            Name = MembershipTerm.Weekly.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.Fortnightly,
            Name = MembershipTerm.Fortnightly.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.Monthly,
            Name = MembershipTerm.Monthly.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.TwoMonths,
            Name = MembershipTerm.TwoMonths.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.Quarterly,
            Name = MembershipTerm.Quarterly.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.FourMonths,
            Name = MembershipTerm.FourMonths.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.FiveMonths,
            Name = MembershipTerm.FiveMonths.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.SixMonths,
            Name = MembershipTerm.SixMonths.ToMembershipTermName(),
        },
        new()
        {
            Type = MembershipTerm.Yearly,
            Name = MembershipTerm.Yearly.ToMembershipTermName(),
        },
    ];

    public IEnumerable<ProductPricingBillingModeDetails> ProductPricingBillingModes =>
    [
        new()
        {
            Type = ProductPricingBillingMode.Upfront,
            Name = ProductPricingBillingMode.Upfront.ToProductPricingBillingModeName(),
        },
        new()
        {
            Type = ProductPricingBillingMode.InArrears,
            Name = ProductPricingBillingMode.InArrears.ToProductPricingBillingModeName(),
        },
    ];

    public IEnumerable<PaymentMethodTypeDetails> PaymentMethods =>
    [
        new()
        {
            Type = PaymentMethod.Card,
            Name = PaymentMethod.Card.ToPaymentMethodName(),
        },
        new()
        {
            Type = PaymentMethod.BankTransfer,
            Name = PaymentMethod.BankTransfer.ToPaymentMethodName(),
        },
    ];

    public IEnumerable<ProductPricingCancellationTypeDetails> ProductPricingCancellationTypes =>
    [
        new()
        {
            Type = ProductPricingCancellationPolicyType.NoCancellation,
            Name = ProductPricingCancellationPolicyType.NoCancellation.ToProductPricingCancellationPolicyTypeName(),
        },
        new()
        {
            Type = ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            Name = ProductPricingCancellationPolicyType.FullRefundBeforeCutoff.ToProductPricingCancellationPolicyTypeName(),
        },
        new()
        {
            Type = ProductPricingCancellationPolicyType.TieredRefund,
            Name = ProductPricingCancellationPolicyType.TieredRefund.ToProductPricingCancellationPolicyTypeName(),
        },
    ];

    public IEnumerable<ProductTypeDetails> ProductTypes =>
    [
        new()
        {
            Type = ProductType.Resource,
            Name = ProductType.Resource.ToProductTypeName(),
        },
        new()
        {
            Type = ProductType.Event,
            Name = ProductType.Event.ToProductTypeName(),
        },
    ];

    public Version MarketplaceVersion()
    {
        var version = versionService.GetVersion();

        return new Version
        {
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision,
        };
    }

    public IEnumerable<CurrencyDetails> Currencies() =>
    [
        new()
        {
            Type = Currency.Nzd,
            Name = Currency.Nzd.ToCurrencyName(),
        },
        new()
        {
            Type = Currency.Usd,
            Name = Currency.Usd.ToCurrencyName(),
        },
    ];
}
