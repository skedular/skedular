using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationOffering : EntityBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool AutoRenew { get; set; }
    public int? UnitPrice { get; set; }
    public int? FixedPrice { get; set; }
    public string Currency { get; set; }
    public int? PurchasedUserCapacity { get; set; }
    public int? PurchasedLocationCapacity { get; set; }
    public int? PurchasedTeamCapacity { get; set; }
    public string? CatalogVersion { get; set; }
    public int DiscountPercentage { get; set; }
    public decimal HostCommissionPercentage { get; set; }
    public DateTimeOffset? SpacesBillingStartsAt { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? OrganizationStripePaymentIntentId { get; set; }
    public virtual OrganizationStripePaymentIntent? OrganizationStripePaymentIntent { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationOfferingConfiguration : IEntityTypeConfiguration<OrganizationOffering>
{
    public void Configure(EntityTypeBuilder<OrganizationOffering> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength).HasDefaultValue(CurrencyConstants.Usd);
        builder.Property(item => item.CatalogVersion).HasMaxLength(Constants.MaxCatalogVersionLength);
        builder.Property(item => item.DiscountPercentage).HasDefaultValue(0);
        builder.Property(item => item.HostCommissionPercentage).HasColumnType("DECIMAL(5,2)").HasDefaultValue(5m);

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationOfferings);
        builder
            .HasOne(item => item.OrganizationStripePaymentIntent)
            .WithOne(item => item.OrganizationOffering)
            .HasForeignKey<OrganizationOffering>(item => item.OrganizationStripePaymentIntentId);

        builder.HasIndex(item => item.Code);
        builder.HasIndex(item => item.Start);
        builder.HasIndex(item => item.End);
        builder.HasIndex(item => new
        {
            item.Start,
            item.End,
        });
        builder.HasIndex(item => item.AutoRenew);
        builder.HasIndex(item => item.UnitPrice);
        builder.HasIndex(item => item.FixedPrice);
        builder.HasIndex(item => item.Currency);
        builder.HasIndex(item => item.PurchasedUserCapacity);
        builder.HasIndex(item => item.PurchasedLocationCapacity);
        builder.HasIndex(item => item.PurchasedTeamCapacity);
        builder.HasIndex(item => item.CatalogVersion);
        builder.HasIndex(item => item.DiscountPercentage);
        builder.HasIndex(item => item.HostCommissionPercentage);
        builder.HasIndex(item => item.SpacesBillingStartsAt);
    }
}
