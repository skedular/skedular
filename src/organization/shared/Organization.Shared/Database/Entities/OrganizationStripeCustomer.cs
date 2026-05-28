using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeCustomer : EntityBaseWithDeleted
{
    public string StripeCustomerId { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationStripeCustomerConfiguration : IEntityTypeConfiguration<OrganizationStripeCustomer>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeCustomer> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeCustomerId).HasMaxLength(Constants.StripeCustomerIdLength);

        builder
            .HasOne(item => item.Organization)
            .WithOne(item => item.OrganizationStripeCustomer)
            .HasForeignKey<OrganizationStripeCustomer>(item => item.OrganizationId);

        builder.HasIndex(item => item.StripeCustomerId);
    }
}
