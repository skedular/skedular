using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripePaymentMethod : EntityBaseWithDeleted
{
    public string? SetupIntentId { get; set; }
    public string? PaymentMethodId { get; set; }
    public string? CardBrand { get; set; }
    public string? CardCountry { get; set; }
    public string? CardDescription { get; set; }
    public byte? CardExpiryMonth { get; set; }
    public short? CardExpiryYear { get; set; }
    public string? CardFingerprint { get; set; }
    public string? CardFunding { get; set; }
    public string? CardIssuer { get; set; }
    public string? CardLastFourDigit { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual ICollection<StripePaymentIntent> StripePaymentIntents { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripePaymentMethodConfiguration : IEntityTypeConfiguration<StripePaymentMethod>
{
    public void Configure(EntityTypeBuilder<StripePaymentMethod> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.SetupIntentId).HasMaxLength(Constants.MaxStripeSetupIntentIdLength);
        builder.Property(item => item.PaymentMethodId).HasMaxLength(Constants.MaxStripePaymentMethodIdLength);
        builder.Property(item => item.CardBrand).HasMaxLength(Constants.MaxStripeCardBrandLength);
        builder.Property(item => item.CardCountry).HasMaxLength(Constants.MaxStripeCardCountryLength);
        builder.Property(item => item.CardDescription).HasMaxLength(Constants.MaxStripeCardDescriptionLength);
        builder.Property(item => item.CardFingerprint).HasMaxLength(Constants.MaxStripeCardFingerprintLength);
        builder.Property(item => item.CardFunding).HasMaxLength(Constants.MaxStripeCardFundingLength);
        builder.Property(item => item.CardIssuer).HasMaxLength(Constants.MaxStripeCardIssuerLength);
        builder.Property(item => item.CardLastFourDigit).HasMaxLength(Constants.MaxStripeCardLastFourDigitLength);

        builder.HasIndex(item => item.SetupIntentId).IsUnique();
        builder.HasIndex(item => item.PaymentMethodId);
        builder.HasIndex(item => new { item.CardExpiryMonth, item.CardExpiryYear });
    }
}
