using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AzureTenantMember : EntityBaseWithDeleted
{
    public string? Email { get; set; }
    public string? Designation { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl64 { get; set; }
    public string? PhotoUrl96 { get; set; }
    public string? PhotoUrl120 { get; set; }
    public string? PhotoUrl240 { get; set; }
    public string? PhotoUrl360 { get; set; }
    public string? PhotoUrl432 { get; set; }
    public string? PhotoUrl504 { get; set; }
    public string? PhotoUrl648 { get; set; }

    public virtual AzureTenant AzureTenant { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TenantMemberConfiguration : IEntityTypeConfiguration<AzureTenantMember>
{
    public void Configure(EntityTypeBuilder<AzureTenantMember> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.Designation).HasMaxLength(Constants.MaxDesignationLength);
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxPersonNameLength);
        builder.Property(item => item.GivenName).HasMaxLength(Constants.MaxGivenNameLength);
        builder.Property(item => item.FamilyName).HasMaxLength(Constants.MaxFamilyNameLength);
        builder.Property(item => item.PreferredLanguage).HasMaxLength(Constants.MaxLocaleLength);
        builder.Property(item => item.PhotoUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl48).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl64).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl96).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl120).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl240).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl360).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl432).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl504).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl648).HasMaxLength(Constants.MaxUrlLength);

        builder
            .HasOne(item => item.AzureTenant)
            .WithMany(item => item.AzureTenantMembers);
    }
}
