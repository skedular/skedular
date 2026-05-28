using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripePaymentIntent : EntityBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; }

    public virtual OrganizationStripePaymentMethod OrganizationStripePaymentMethod { get; set; }
    public virtual OrganizationOffering? OrganizationOffering { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationStripePaymentIntentConfiguration : IEntityTypeConfiguration<OrganizationStripePaymentIntent>
{
    public void Configure(EntityTypeBuilder<OrganizationStripePaymentIntent> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);

        builder.HasOne(item => item.OrganizationStripePaymentMethod).WithMany(item => item.OrganizationStripePaymentIntents);

        builder.HasIndex(item => item.Amount);
        builder.HasIndex(item => item.Currency);
    }
}
