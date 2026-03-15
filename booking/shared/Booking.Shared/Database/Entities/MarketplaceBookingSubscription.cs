using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBookingSubscription : EntityBaseWithDeleted
{
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public DateTimeOffset NextRenewalAt { get; set; }
    public string Status { get; set; }
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public ProductPricing ProductPricing { get; set; }

    public virtual ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public virtual ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public virtual ICollection<Team> InvolvedTeams { get; set; } = [];
    public virtual Customer? CreatedByCustomer { get; set; }
    public virtual Customer? LastModifiedByCustomer { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }
    public virtual ProductVersion ProductVersion { get; set; }
    public virtual ICollection<RecurringBooking> RecurringBookings { get; set; } = [];
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingSubscriptionConfiguration : IEntityTypeConfiguration<MarketplaceBookingSubscription>
{
    public void Configure(EntityTypeBuilder<MarketplaceBookingSubscription> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Status).HasMaxLength(Constants.MaxMarketplaceBookingSubscriptionStatusLength);
        builder.Property(item => item.ProductPricing).HasColumnType("jsonb");

        builder.HasMany(item => item.InvolvedCustomers).WithMany(item => item.InvolvedMarketplaceBookingSubscription);
        builder.HasMany(item => item.InvolvedOrganizations).WithMany(item => item.InvolvedMarketplaceBookingSubscription);
        builder.HasMany(item => item.InvolvedTeams).WithMany(item => item.InvolvedMarketplaceBookingSubscription);
        builder.HasOne(item => item.CreatedByCustomer).WithMany(item => item.CreatedMarketplaceBookingSubscriptions);
        builder.HasOne(item => item.LastModifiedByCustomer).WithMany(item => item.LastModifiedMarketplaceBookingSubscriptions);
        builder.HasOne(item => item.DeletedByCustomer).WithMany(item => item.DeletedMarketplaceBookingSubscriptions);
        builder.HasOne(item => item.ProductVersion).WithMany(item => item.MarketplaceBookingSubscriptions);

        builder.HasIndex(item => item.StartedAt);
        builder.HasIndex(item => item.CancelledAt);
        builder.HasIndex(item => item.NextRenewalAt);
        builder.HasIndex(item => item.Status);
        builder.Property(item => item.AutoRenew);
        builder.Property(item => item.CancelAtPeriodEnd);
    }
}
