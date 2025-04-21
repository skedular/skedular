using Api.Shared;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public string? About { get; set; }
    public string? Website { get; set; }
    public bool AgreedToTermsOfUse { get; set; }
    public string? LogoUrl { get; set; }
    public string Type { get; set; }
    public string MemberVisibilityPolicy { get; set; }
    public bool HasAttachedPaymentMethod { get; set; }
    public DateTimeOffset? PaymentMethodEventRaisedAt { get; set; }
    public DateTimeOffset? DailyMemberCountLastRecordedAt { get; set; }
    public string? ContactEmail { get; set; }

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual TermsOfUse? TermsOfUse { get; set; }
    public virtual ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<DailyMemberCountRecording> DailyMemberCountRecordings { get; set; } = [];
    public virtual ICollection<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
    public virtual ICollection<Location> Locations { get; set; } = [];
    public virtual ICollection<Team> Teams { get; set; } = [];
    public virtual ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public virtual ICollection<AzureTenant> AzureTenants { get; set; } = [];
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = [];

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? PhysicalAddressId { get; set; }
    public virtual Address? PhysicalAddress { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.About).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.Website).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.LogoUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxOrganizationTypeLength).HasDefaultValue(OrganizationTypeConstants.Private);
        builder
            .Property(item => item.MemberVisibilityPolicy)
            .HasMaxLength(Constants.MaxOrganizationMemberVisibilityPolicyLength)
            .HasDefaultValue(OrganizationMemberVisibilityPolicyConstants.FullAccess);
        builder.Property(item => item.ContactEmail).HasMaxLength(Constants.MaxEmailLength);

        builder.HasOne(item => item.TermsOfUse).WithMany(item => item.Organizations);
        builder.HasMany(item => item.IndustrySubCategories).WithMany(item => item.Organizations);
        builder.HasOne(item => item.PhysicalAddress).WithOne(item => item.Organization).HasForeignKey<Organization>(item => item.PhysicalAddressId);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.About);
        builder.HasIndex(item => item.Website);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.HasAttachedPaymentMethod);
        builder.HasIndex(item => item.PaymentMethodEventRaisedAt);
        builder.HasIndex(item => item.DailyMemberCountLastRecordedAt);
    }
}
