using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Customer : ReplicatedEntityBaseWithDeleted
{
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }

    public string? BillingContactCompanyName { get; set; }
    public string? BillingContactEmail { get; set; }
    public string? BillingContactAddressLine1 { get; set; }
    public string? BillingContactAddressLine2 { get; set; }
    public string? BillingContactSuburb { get; set; }
    public string? BillingContactCity { get; set; }
    public string? BillingContactProvince { get; set; }
    public string? BillingContactZipcode { get; set; }
    public string? BillingContactCountry { get; set; }

    public virtual ICollection<Identity> Identities { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Title).HasMaxLength(Constants.MaxPersonTitleLength);
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxPersonNameLength);
        builder.Property(item => item.GivenName).HasMaxLength(Constants.MaxGivenNameLength);
        builder.Property(item => item.MiddleName).HasMaxLength(Constants.MaxMiddleNameLength);
        builder.Property(item => item.FamilyName).HasMaxLength(Constants.MaxFamilyNameLength);

        builder.Property(item => item.BillingContactCompanyName).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.BillingContactEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.BillingContactAddressLine1).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.BillingContactAddressLine2).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.BillingContactSuburb).HasMaxLength(Constants.MaxSuburbLength);
        builder.Property(item => item.BillingContactCity).HasMaxLength(Constants.MaxCityLength);
        builder.Property(item => item.BillingContactProvince).HasMaxLength(Constants.MaxProvinceLength);
        builder.Property(item => item.BillingContactZipcode).HasMaxLength(Constants.MaxZipcodeLength);
        builder.Property(item => item.BillingContactCountry).HasMaxLength(Constants.MaxCountryLength);
    }
}
