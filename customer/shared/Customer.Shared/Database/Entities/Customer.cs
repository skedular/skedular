using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Customer : EntityBaseWithDeleted
{
    public string? Designation { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }
    public string? Timezone { get; set; }
    public string? Locale { get; set; }
    public bool? IsOrganizationOnboardingDone { get; set; }
    public bool? IsLocationOnboardingDone { get; set; }
    public bool? IsDefaultOrganizationOnboardingDone { get; set; }
    public bool? IsDefaultLocationOnboardingDone { get; set; }
    public bool? IsPreferredZoneOnboardingDone { get; set; }
    public bool? IsPreferredDeskOnboardingDone { get; set; }

    public virtual ICollection<Identity> Identities { get; set; } = [];
    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = [];
    public virtual Organization? DefaultOrganization { get; set; }
    public virtual ICollection<Location> DefaultLocations { get; set; } = [];
    public virtual ICollection<LocationTag> PreferredLocationTags { get; set; } = [];
    public virtual ICollection<Desk> PreferredDesks { get; set; } = [];
    public virtual ICollection<Team> DefaultTeams { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<LocationMember> LocationMemberships { get; set; } = [];
    public virtual ICollection<TeamMember> TeamMemberships { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Designation).HasMaxLength(Constants.MaxDesignationLength);
        builder.Property(item => item.Title).HasMaxLength(Constants.MaxTitleLength);
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxPersonNameLength);
        builder.Property(item => item.GivenName).HasMaxLength(Constants.MaxGivenNameLength);
        builder.Property(item => item.MiddleName).HasMaxLength(Constants.MaxMiddleNameLength);
        builder.Property(item => item.FamilyName).HasMaxLength(Constants.MaxFamilyNameLength);
        builder.Property(item => item.PhotoUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl24).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl32).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl48).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl72).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl192).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl512).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.Timezone).HasMaxLength(Constants.MaxTimezoneLength);
        builder.Property(item => item.Locale).HasMaxLength(Constants.MaxLocaleLength);

        builder
            .HasOne(item => item.DefaultOrganization)
            .WithMany(item => item.DefaultedByCustomers);

        builder
            .HasMany(item => item.DefaultLocations)
            .WithMany(item => item.DefaultedByCustomers);

        builder
            .HasMany(item => item.PreferredLocationTags)
            .WithMany(item => item.PreferredByCustomers);

        builder
            .HasMany(item => item.PreferredDesks)
            .WithMany(item => item.PreferredByCustomers);

        builder
            .HasMany(item => item.DefaultTeams)
            .WithMany(item => item.DefaultedByCustomers);

        builder.HasIndex(item => item.Designation);
        builder.HasIndex(item => item.Title);
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.GivenName);
        builder.HasIndex(item => item.MiddleName);
        builder.HasIndex(item => item.FamilyName);
        builder.HasIndex(item => item.Timezone);
        builder.HasIndex(item => item.Locale);
    }
}
