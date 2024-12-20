using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Desk : ReplicatedEntityBase
{
    public string? Name { get; set; }

    public virtual Location Location { get; set; }
    public virtual ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class DeskConfiguration : IEntityTypeConfiguration<Desk>
{
    public void Configure(EntityTypeBuilder<Desk> builder)
    {
        builder.ConfigureReplicatedEntityBase();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxDeskNameLength);

        builder
            .HasOne(item => item.Location)
            .WithMany(item => item.Desks);

        builder.HasIndex(item => item.Name);
    }
}
