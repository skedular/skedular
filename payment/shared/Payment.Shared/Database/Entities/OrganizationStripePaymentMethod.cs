using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Shared.Models;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripePaymentMethod : EntityBaseWithDeleted
{
    public string? SetupIntentId { get; set; }
    public string? ClientSecret { get; set; }
    public OrganizationStripePaymentMethodStatus Status { get; set; }
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

    public virtual Organization Organization { get; set; }

    public virtual ICollection<OrganizationOfferingStripePaymentIntent> OrganizationOfferingStripePaymentIntents
    {
        get;
        set;
    } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationStripePaymentMethodConfiguration : IEntityTypeConfiguration<OrganizationStripePaymentMethod>
{
    public void Configure(EntityTypeBuilder<OrganizationStripePaymentMethod> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.OrganizationStripePaymentMethods);

        builder.HasIndex(item => item.SetupIntentId).IsUnique();
        builder.HasIndex(item => item.ClientSecret);
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => item.PaymentMethodId);
        builder.HasIndex(item => new { item.CardExpiryMonth, item.CardExpiryYear });
    }
}
