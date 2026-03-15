using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }
    public string Type { get; set; }
    public string BillingCycle { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool? IsOwnershipVerified { get; set; }

    public virtual ICollection<OrganizationTag> Tags { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<Location> Locations { get; set; } = [];
    public virtual ICollection<Team> Teams { get; set; } = [];
    public virtual ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public virtual ICollection<Product> Products { get; set; } = [];
    public virtual ICollection<Booking> InvolvedBookings { get; set; } = [];
    public virtual ICollection<RecurringBooking> InvolvedRecurringBooking { get; set; } = [];
    public virtual ICollection<MarketplaceBooking> PaidMarketplaceBookings { get; set; } = [];
    public virtual ICollection<StripeCustomer> StripeCustomers { get; set; } = [];
    public virtual ICollection<OrganizationInvoiceCounter> OrganizationInvoiceCounters { get; set; } = [];
    public virtual ICollection<MarketplaceBookingSubscription> InvolvedMarketplaceBookingSubscription { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.UniqueAlphanumericName).HasMaxLength(Constants.MaxOrganizationUniqueAlphanumericNameLength);
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.LogoUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.Offering).HasColumnType("jsonb");
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxOrganizationTypeLength).HasDefaultValue(OrganizationTypeConstants.Private);
        builder.Property(item => item.BillingCycle)
            .HasMaxLength(Constants.MaxOrganizationBillingCycleLength)
            .HasDefaultValue(OrganizationBillingCycleConstants.Monthly);
        builder.Property(item => item.ContactEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.ContactPhone).HasMaxLength(Constants.MaxPhoneNumberLength);

        builder.HasIndex(item => item.UniqueAlphanumericName).IsUnique();
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.BillingCycle);
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.IsOwnershipVerified);
    }
}
