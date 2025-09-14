using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public Offering? Offering { get; set; }
    public string Type { get; set; }

    public virtual ICollection<OrganizationTag> Tags { get; set; } = [];
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<Product> Products { get; set; } = [];
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.UniqueAlphanumericName).HasMaxLength(Constants.MaxOrganizationUniqueAlphanumericNameLength);
        builder.Property(item => item.Offering).HasColumnType("jsonb");
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxOrganizationTypeLength).HasDefaultValue(OrganizationTypeConstants.Private);

        builder.HasIndex(item => item.UniqueAlphanumericName).IsUnique();
        builder.HasIndex(item => item.Type);
    }
}
