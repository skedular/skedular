using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AccountingContactLink : EntityBase
{
    public string Provider { get; set; }
    public string LocalEntityType { get; set; }
    public string LocalEntityId { get; set; }
    public string? ExternalContactId { get; set; }
    public string? ExternalContactName { get; set; }
    public DateTimeOffset? LastSyncedAt { get; set; }
    public string? LastError { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class AccountingContactLinkConfiguration : IEntityTypeConfiguration<AccountingContactLink>
{
    public void Configure(EntityTypeBuilder<AccountingContactLink> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Provider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.LocalEntityType).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.LocalEntityId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.ExternalContactId).HasMaxLength(Constants.MaxExternalContactIdLength);
        builder.Property(item => item.ExternalContactName).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);

        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => new
        {
            item.OrganizationId,
            item.Provider,
            item.LocalEntityType,
            item.LocalEntityId,
        }).IsUnique();
        builder.HasIndex(item => new
        {
            item.OrganizationId,
            item.Provider,
            item.ExternalContactId,
        }).IsUnique();
    }
}
