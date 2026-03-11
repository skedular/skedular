using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : EntityBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string Name { get; set; }
    public string? Website { get; set; }
    public bool AgreedToTermsOfUse { get; set; }
    public string? LogoUrl { get; set; }
    public string Type { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public ICollection<CdnImageFile>? FeatureImages { get; set; }
    public ListingMetadata? ListingMetadata { get; set; }
    public ListingMetadata? MarketplaceListingMetadata { get; set; }

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual TermsOfUse? TermsOfUse { get; set; }
    public virtual ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public virtual ICollection<DailyMemberCountRecording> DailyMemberCountRecordings { get; set; } = [];
    public virtual ICollection<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
    public virtual ICollection<Location> Locations { get; set; } = [];
    public virtual ICollection<Team> Teams { get; set; } = [];
    public virtual ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public virtual ICollection<AzureTenant> AzureTenants { get; set; } = [];
    public virtual OrganizationSsoSettings? OrganizationSsoSettings { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = [];
    public virtual ICollection<Booking> InvolvedBookings { get; set; } = [];
    public virtual ICollection<OrganizationStripePaymentMethod> OrganizationStripePaymentMethods { get; set; } = [];
    public virtual OrganizationStripeCustomer? OrganizationStripeCustomer { get; set; }
    public virtual OrganizationBillingDetails? BillingDetails { get; set; }
    public virtual ICollection<OrganizationStripeConnectAccount> OrganizationStripeConnectAccounts { get; set; } = [];
    public virtual ICollection<OrganizationBankAccount> OrganizationBankAccounts { get; set; } = [];
    public virtual OrganizationTaxDetails? OrganizationTaxDetails { get; set; }
    public virtual OrganizationPhysicalAddress? PhysicalAddress { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.UniqueAlphanumericName).HasMaxLength(Constants.MaxOrganizationUniqueAlphanumericNameLength);
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.Website).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.LogoUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxOrganizationTypeLength).HasDefaultValue(OrganizationTypeConstants.Private);
        builder.Property(item => item.ContactEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.ContactPhone).HasMaxLength(Constants.MaxPhoneNumberLength);
        builder.Property(item => item.FeatureImages).HasColumnType("jsonb");
        builder.Property(item => item.ListingMetadata).HasColumnType("jsonb");
        builder.Property(item => item.MarketplaceListingMetadata).HasColumnType("jsonb");

        builder.HasOne(item => item.TermsOfUse).WithMany(item => item.Organizations);
        builder.HasMany(item => item.IndustrySubCategories).WithMany(item => item.Organizations);
        builder.HasMany(item => item.OrganizationStripePaymentMethods).WithOne(item => item.Organization);

        builder.HasIndex(item => item.UniqueAlphanumericName).IsUnique();
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Website);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.IsOwnershipVerified);
    }
}
