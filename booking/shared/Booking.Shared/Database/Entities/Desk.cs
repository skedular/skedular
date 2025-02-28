using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Desk : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
    public bool Deactivated { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }

    public virtual Location? Location { get; set; }
    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public virtual ICollection<Customer> PreferredByCustomers { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class DeskConfiguration : IEntityTypeConfiguration<Desk>
{
    public void Configure(EntityTypeBuilder<Desk> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxDeskNameLength);
        builder.Property(item => item.Deactivated).HasDefaultValue(false);
        builder.Property(item => item.RequireBookingApproval).HasDefaultValue(false);
        builder.Property(item => item.Color).HasMaxLength(Constants.MaxColorValueLength);

        builder.HasOne(item => item.Location).WithMany(item => item.Desks);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.TaggedDesks);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Deactivated);
        builder.HasIndex(item => item.RequireBookingApproval);
    }
}
