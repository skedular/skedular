using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationXeroConnection : EntityBase
{
    public string TenantId { get; set; }
    public string TenantName { get; set; }
    public string BillingMode { get; set; }
    public string? Scopes { get; set; }
    public bool IsActive { get; set; }
    public bool SendInvoicesViaXero { get; set; }
    public bool AutoReconcilePayments { get; set; }
    public string? DefaultSalesAccountCode { get; set; }
    public string? DefaultReceivablesAccountCode { get; set; }
    public string? DefaultTrackingCategory1 { get; set; }
    public string? DefaultTrackingCategory2 { get; set; }
    public string? DefaultBrandingThemeId { get; set; }
    public string? DefaultReferencePrefix { get; set; }
    public string? AccessTokenEncrypted { get; set; }
    public string? RefreshTokenEncrypted { get; set; }
    public DateTimeOffset? AccessTokenExpiresAt { get; set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
    public DateTimeOffset? LastSuccessfulSyncAt { get; set; }
    public string? LastError { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationXeroConnectionConfiguration : IEntityTypeConfiguration<OrganizationXeroConnection>
{
    public void Configure(EntityTypeBuilder<OrganizationXeroConnection> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.TenantId).HasMaxLength(Constants.MaxXeroTenantIdLength);
        builder.Property(item => item.TenantName).HasMaxLength(Constants.MaxTenantNameLength);
        builder.Property(item => item.BillingMode)
            .HasMaxLength(Constants.MaxXeroBillingModeLength)
            .HasDefaultValue(XeroBillingModeConstants.Disabled);
        builder.Property(item => item.Scopes).HasMaxLength(Constants.MaxAccountingScopesLength);
        builder.Property(item => item.IsActive).HasDefaultValue(false);
        builder.Property(item => item.SendInvoicesViaXero).HasDefaultValue(true);
        builder.Property(item => item.AutoReconcilePayments).HasDefaultValue(true);
        builder.Property(item => item.DefaultSalesAccountCode).HasMaxLength(Constants.MaxAccountingAccountCodeLength);
        builder.Property(item => item.DefaultReceivablesAccountCode).HasMaxLength(Constants.MaxAccountingAccountCodeLength);
        builder.Property(item => item.DefaultTrackingCategory1).HasMaxLength(Constants.MaxTagNameLength);
        builder.Property(item => item.DefaultTrackingCategory2).HasMaxLength(Constants.MaxTagNameLength);
        builder.Property(item => item.DefaultBrandingThemeId).HasMaxLength(Constants.MaxBrandingThemeLength);
        builder.Property(item => item.DefaultReferencePrefix).HasMaxLength(Constants.MaxAccountingReferencePrefixLength);
        builder.Property(item => item.AccessTokenEncrypted).HasMaxLength(Constants.MaxEncryptedTokenLength);
        builder.Property(item => item.RefreshTokenEncrypted).HasMaxLength(Constants.MaxEncryptedTokenLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);

        builder
            .HasOne(item => item.Organization)
            .WithOne(item => item.OrganizationXeroConnection)
            .HasForeignKey<OrganizationXeroConnection>(item => item.OrganizationId);

        builder.HasIndex(item => item.OrganizationId).IsUnique();
        builder.HasIndex(item => item.TenantId);
        builder.HasIndex(item => item.BillingMode);
        builder.HasIndex(item => item.IsActive);
    }
}
