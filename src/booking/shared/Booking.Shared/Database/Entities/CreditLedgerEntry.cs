using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Constants = Api.Shared.Services.Constants;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CreditLedgerEntry : EntityBase
{
    public int Quantity { get; set; }
    public string TransactionType { get; set; }
    public string ReferenceKey { get; set; }
    public string? ActorOrSource { get; set; }
    public CreditLedgerEntryMetadata? Metadata { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string EntitlementId { get; set; }
    public virtual Entitlement Entitlement { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? BookingId { get; set; }
    public virtual Booking? Booking { get; set; }

    public virtual Booking? ConsumingBooking { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CreditLedgerEntryConfiguration : IEntityTypeConfiguration<CreditLedgerEntry>
{
    public void Configure(EntityTypeBuilder<CreditLedgerEntry> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.ReferenceKey).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.ActorOrSource).HasMaxLength(Constants.MaxCreditLedgerActorOrSourceLength);
        builder.Property(item => item.TransactionType).HasMaxLength(Constants.MaxCreditLedgerTransactionTypeLength);
        builder.Property(item => item.Metadata).HasColumnType("jsonb");

        builder.HasOne(item => item.Entitlement).WithMany(item => item.LedgerEntries).HasForeignKey(item => item.EntitlementId);
        builder.HasOne(item => item.Booking).WithMany().HasForeignKey(item => item.BookingId);

        builder.HasIndex(item => new
        {
            item.EntitlementId,
            item.ReferenceKey,
        }).IsUnique();
        builder.HasIndex(item => item.BookingId);
    }
}
