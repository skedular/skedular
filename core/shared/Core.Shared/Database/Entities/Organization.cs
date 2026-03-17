using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public string Type { get; set; }
    public bool? IsOwnershipVerified { get; set; }

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.CustomDomain).HasMaxLength(Constants.MaxOrganizationCustomDomainLength);
        builder
            .Property(item => item.Type)
            .HasMaxLength(Constants.MaxOrganizationTypeLength)
            .HasDefaultValue(OrganizationTypeConstants.Private);

        builder.HasIndex(item => item.CustomDomain).IsUnique();
        builder.HasIndex(item => item.IsOwnershipVerified);
    }
}
