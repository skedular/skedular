using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Customer : ReplicatedEntityBaseWithDeleted
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripeCustomerId { get; set; }
    public virtual StripeCustomer? StripeCustomer { get; set; }

    public virtual ICollection<Identity> Identities { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<StripePaymentMethod> StripePaymentMethods { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.HasOne(item => item.StripeCustomer).WithOne(item => item.Customer).HasForeignKey<Customer>(item => item.StripeCustomerId);
        builder.HasMany(item => item.StripePaymentMethods).WithOne(item => item.Customer);
    }
}
