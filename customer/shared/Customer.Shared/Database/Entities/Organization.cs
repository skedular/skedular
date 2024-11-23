using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }

    public virtual ICollection<OrganizationTag> Tags { get; set; } = [];
    public virtual ICollection<Location> Locations { get; set; } = [];
    public virtual ICollection<Team> Teams { get; set; } = [];
    public virtual ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.LogoUrl).HasMaxLength(Constants.MaxUrlLength);

        builder.HasIndex(item => item.Name);
    }
}
