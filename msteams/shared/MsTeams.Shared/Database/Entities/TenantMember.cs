using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TenantMember : EntityBaseWithDeleted
{
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? PrincipalName { get; set; }

    public virtual Tenant Tenant { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TenantMemberConfiguration : IEntityTypeConfiguration<TenantMember>
{
    public void Configure(EntityTypeBuilder<TenantMember> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.GivenName).HasMaxLength(Constants.MaxGivenNameLength);
        builder.Property(item => item.Surname).HasMaxLength(Constants.MaxFamilyNameLength);
        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.JobTitle).HasMaxLength(Constants.MaxDesignationLength);
        builder.Property(item => item.PreferredLanguage).HasMaxLength(Constants.MaxLocaleLength);
        builder.Property(item => item.PrincipalName).HasMaxLength(Constants.MaxTenantPrincipalLength);

        builder
            .HasOne(item => item.Tenant)
            .WithMany(item => item.TenantMembers);
    }
}
