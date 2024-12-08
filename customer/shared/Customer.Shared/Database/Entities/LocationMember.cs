using Api.Shared;
using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationMember : ReplicatedEntityBaseWithDeleted
{
    public OldLocationMembershipType? MembershipType { get; set; }
    public string? NewMembershipType { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string LocationId { get; set; } = string.Empty;
    public virtual Location Location { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; } = string.Empty;
    public virtual Customer Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationMemberConfiguration : IEntityTypeConfiguration<LocationMember>
{
    public void Configure(EntityTypeBuilder<LocationMember> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder
            .Property(item => item.NewMembershipType)
            .HasMaxLength(Constants.MaxMembershipTypeLength)
            .HasComputedColumnSql(@"
                    CASE 
                        WHEN ""MembershipType"" = 0 THEN 'OWNER'
                        WHEN ""MembershipType"" = 1 THEN 'ADMINISTRATOR'
                        WHEN ""MembershipType"" = 2 THEN 'MEMBER'
                        ELSE 'UNKNOWN'
                    END", true);
        
        builder
            .HasOne(item => item.Location)
            .WithMany(item => item.LocationMembers)
            .HasForeignKey(item => item.LocationId);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.LocationMemberships)
            .HasForeignKey(item => item.CustomerId);

        builder.HasIndex(item => item.MembershipType);
        builder.HasIndex(item => new { item.CustomerId, item.LocationId }).IsUnique();
    }
}
