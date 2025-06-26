using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeCustomer : EntityBaseWithDeleted
{
    public string StripeCustomerId { get; set; }
    public string StripeAccountId { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual ICollection<StripeCheckoutSession> StripeCheckoutSessions { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeCustomerConfiguration : IEntityTypeConfiguration<StripeCustomer>
{
    public void Configure(EntityTypeBuilder<StripeCustomer> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeCustomerId).HasMaxLength(Constants.StripeCustomerIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxStripeConnectAccountIdLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.StripeCustomers);
        builder.HasOne(item => item.Customer).WithMany(item => item.StripeCustomers);

        builder.HasIndex(item => item.StripeCustomerId);
        builder.HasIndex(item => item.StripeAccountId);
    }
}
