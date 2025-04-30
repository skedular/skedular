using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeCustomer : EntityBaseWithDeleted
{
    public string StripeCustomerId { get; set; }
    public virtual Organization? Organization { get; set; }
    public virtual Customer? Customer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripeConnectAccountId { get; set; }
    public virtual StripeConnectAccount? StripeConnectAccount { get; set; }

    public virtual ICollection<StripeCheckoutSession> StripeCheckoutSessions { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeCustomerConfiguration : IEntityTypeConfiguration<StripeCustomer>
{
    public void Configure(EntityTypeBuilder<StripeCustomer> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeCustomerId).HasMaxLength(Constants.StripeCustomerIdLength);

        builder.HasOne(item => item.StripeConnectAccount).WithMany(item => item.StripeCustomers);

        builder.HasIndex(item => item.StripeCustomerId);
    }
}
