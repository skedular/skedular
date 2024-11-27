using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationTag : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Type { get; set; }

    public virtual Location Location { get; set; }
    public virtual ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationTagConfiguration : IEntityTypeConfiguration<LocationTag>
{
    public void Configure(EntityTypeBuilder<LocationTag> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTagNameLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxTagTypeLength);

        builder
            .HasOne(item => item.Location)
            .WithMany(item => item.Tags);

        builder.HasIndex(item => item.Name);
    }
}
