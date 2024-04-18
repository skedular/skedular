using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationOfferingStripePaymentIntent : EntityBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; }

    public virtual OrganizationStripePaymentMethod OrganizationStripePaymentMethod { get; set; }
    public virtual OrganizationOffering OrganizationOffering { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class
    OrganizationOfferingStripePaymentIntentConfiguration : IEntityTypeConfiguration<
    OrganizationOfferingStripePaymentIntent>
{
    public void Configure(EntityTypeBuilder<OrganizationOfferingStripePaymentIntent> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);

        builder
            .HasOne(item => item.OrganizationStripePaymentMethod)
            .WithMany(item => item.OrganizationOfferingStripePaymentIntents);

        builder
            .HasOne(item => item.OrganizationOffering)
            .WithMany(item => item.OrganizationOfferingStripePaymentIntents);
    }
}
