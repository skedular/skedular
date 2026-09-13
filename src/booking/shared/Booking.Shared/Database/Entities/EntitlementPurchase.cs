using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class EntitlementPurchase : EntityBase
{
    /// <summary>
    ///     The customer chose Stripe Billing for this marketplace purchase. This is purchase-level
    ///     authorization, not a Customer-profile payment-method reference.
    /// </summary>
    public bool AutoRenew { get; set; }

    public string PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }
    public DateTimeOffset? PaymentConfirmedAt { get; set; }
    public DateTimeOffset PaymentExpiry { get; set; }
    public DateTimeOffset ServiceStartAt { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public ProductPricing ProductPricing { get; set; }
    public string? CheckoutReturnUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? PaymentInstructions { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripeCheckoutUrl { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeAccountId { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripePriceId { get; set; }
    public string? StripeSubscriptionStatus { get; set; }
    public DateTimeOffset? StripeCurrentPeriodEndsAt { get; set; }
    public bool StripeCancelAtPeriodEnd { get; set; }
    public ICollection<string> InvoiceEmailList { get; set; } = [];

    public string? FailureReason { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductVersionId { get; set; }
    public virtual ProductVersion ProductVersion { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? EntitlementId { get; set; }
    public virtual Entitlement? Entitlement { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public sealed class EntitlementPurchaseConfiguration : IEntityTypeConfiguration<EntitlementPurchase>
{
    public void Configure(EntityTypeBuilder<EntitlementPurchase> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.PaymentStatus).HasMaxLength(Constants.MaxBookingPaymentStatusLength);
        builder.Property(item => item.PaymentMethod).HasMaxLength(Constants.MaxBookingMethodLength);
        builder.Property(item => item.FailureReason).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.Amount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.ProductPricing).HasColumnType("jsonb");
        builder.Property(item => item.CheckoutReturnUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.InvoiceNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.InvoiceUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PaymentInstructions).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.StripeCheckoutSessionId).HasMaxLength(Constants.MaxStripeCheckoutSessionIdLength);
        builder.Property(item => item.StripeCheckoutUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.StripePaymentIntentId).HasMaxLength(Constants.MaxRefundStripePaymentIntentIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxRefundStripeAccountIdLength);
        builder.Property(item => item.StripeCustomerId).HasMaxLength(Constants.StripeCustomerIdLength);
        builder.Property(item => item.StripeSubscriptionId).HasMaxLength(256);
        builder.Property(item => item.StripePriceId).HasMaxLength(Constants.MaxStripePriceIdLength);
        builder.Property(item => item.StripeSubscriptionStatus).HasMaxLength(64);
        builder.Property(item => item.InvoiceEmailList).HasColumnType("jsonb");

        builder.HasOne(item => item.Customer).WithMany().HasForeignKey(item => item.CustomerId);
        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.ProductVersion).WithMany().HasForeignKey(item => item.ProductVersionId);
        builder.HasOne(item => item.Entitlement)
            .WithOne(item => item.EntitlementPurchase)
            .HasForeignKey<EntitlementPurchase>(item => item.EntitlementId);

        builder.HasIndex(item => item.PaymentStatus);
        builder.HasIndex(item => item.PaymentExpiry);
        builder.HasIndex(item => item.CustomerId);
        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => item.EntitlementId).IsUnique();
        builder.HasIndex(item => new
        {
            item.StripeAccountId,
            item.StripeSubscriptionId,
        }).IsUnique();
    }
}
