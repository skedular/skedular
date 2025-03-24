using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Customer : ReplicatedEntityBaseWithDeleted
{
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

    public virtual ICollection<Identity> Identities { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<LocationMember> LocationMembers { get; set; } = [];
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual Organization? DefaultOrganization { get; set; }
    public virtual ICollection<Location> PreferredLocations { get; set; } = [];
    public virtual ICollection<Resource> PreferredResources { get; set; } = [];
    public virtual ICollection<Team> PreferredTeams { get; set; } = [];
    public virtual ICollection<OrganizationTag> PreferredOrganizationTags { get; set; } = [];
    public virtual ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

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

        builder.HasOne(item => item.DefaultOrganization).WithMany(item => item.DefaultedByCustomers);
        builder.HasMany(item => item.PreferredLocations).WithMany(item => item.PreferredByCustomers);
        builder.HasMany(item => item.PreferredResources).WithMany(item => item.PreferredByCustomers);
        builder.HasMany(item => item.PreferredTeams).WithMany(item => item.PreferredByCustomers);
        builder.HasMany(item => item.PreferredOrganizationTags).WithMany(item => item.PreferredByCustomers);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.GivenName);
        builder.HasIndex(item => item.MiddleName);
        builder.HasIndex(item => item.FamilyName);
    }
}
