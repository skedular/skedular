using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeCustomer : EntityBase
{
    public string StripeCustomerId { get; set; }
    public virtual Organization? Organization { get; set; }
    public virtual Customer? Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeCustomerConfiguration : IEntityTypeConfiguration<StripeCustomer>
{
    public void Configure(EntityTypeBuilder<StripeCustomer> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.StripeCustomerId).HasMaxLength(Constants.StripeCustomerIdLength);

        builder.HasIndex(item => item.StripeCustomerId);
    }
}
