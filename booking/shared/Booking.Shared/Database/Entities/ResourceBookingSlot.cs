using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ResourceBookingSlot : EntityBase
{
    public DateTimeOffset Start { get; set; }
    public bool Available { get; set; }

    public virtual Resource Resource { get; set; }
    public virtual ICollection<Customer> Customers { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ResourceBookingSlotConfiguration : IEntityTypeConfiguration<ResourceBookingSlot>
{
    public void Configure(EntityTypeBuilder<ResourceBookingSlot> builder)
    {
        builder.ConfigureEntityBase();

        builder.HasOne(item => item.Resource).WithMany(item => item.ResourceBookingSlots);
        builder.HasMany(item => item.Customers).WithMany(item => item.ResourceBookingSlots);

        builder.HasIndex(item => item.Start);
        builder.HasIndex(item => item.Available);
    }
}
