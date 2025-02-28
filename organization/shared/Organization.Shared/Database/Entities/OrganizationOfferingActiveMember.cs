using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationOfferingActiveMember : EntityBase
{
    public virtual OrganizationMember OrganizationMember { get; set; }
    public virtual OrganizationOffering OrganizationOffering { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationOfferingActiveMemberConfiguration : IEntityTypeConfiguration<OrganizationOfferingActiveMember>
{
    public void Configure(EntityTypeBuilder<OrganizationOfferingActiveMember> builder)
    {
        builder.ConfigureEntityBase();

        builder.HasOne(item => item.OrganizationMember).WithMany(item => item.OrganizationOfferingActiveMembers);
        builder.HasOne(item => item.OrganizationOffering).WithMany(item => item.OrganizationOfferingActiveMembers);
    }
}
