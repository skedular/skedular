using Api.Shared;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? StripeCustomerId { get; set; }
    public string Type { get; set; }
    public string MemberVisibilityPolicy { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? PhysicalAddressId { get; set; }
    public virtual Address? PhysicalAddress { get; set; }

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public virtual ICollection<OrganizationStripePaymentMethod> OrganizationStripePaymentMethods { get; set; } = [];
    public virtual ICollection<OrganizationStripeConnectAccount> OrganizationStripeConnectAccounts { get; set; } = [];
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public virtual ICollection<Product> Products { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.StripeCustomerId).HasMaxLength(Constants.StripeCustomerIdLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxOrganizationTypeLength).HasDefaultValue(OrganizationTypeConstants.Private);
        builder
            .Property(item => item.MemberVisibilityPolicy)
            .HasMaxLength(Constants.MaxOrganizationMemberVisibilityPolicyLength)
            .HasDefaultValue(OrganizationMemberVisibilityPolicyConstants.FullAccess);
        builder.Property(item => item.ContactEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.ContactPhone).HasMaxLength(Constants.MaxPhoneNumberLength);

        builder.HasOne(item => item.PhysicalAddress).WithOne(item => item.Organization).HasForeignKey<Organization>(item => item.PhysicalAddressId);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.StripeCustomerId);
    }
}
