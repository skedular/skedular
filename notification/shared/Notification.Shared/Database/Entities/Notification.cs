using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Shared.Models;

namespace Notification.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Notification : EntityBaseWithDeleted
{
    public DateTimeOffset EventRaisedAt { get; set; }
    public string SourceId { get; set; }
    public NotificationType Type { get; set; }

    public virtual Customer? InvitedBy { get; set; }
    public virtual Customer? Invitee { get; set; }
    public virtual Organization? Organization { get; set; }
    public virtual Location? Location { get; set; }
    public virtual Team? Team { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.SourceId).HasMaxLength(Constants.MaxUniqueIdLength);

        builder
            .HasOne(item => item.InvitedBy)
            .WithMany(item => item.InvitedByNotifications);

        builder
            .HasOne(item => item.Invitee)
            .WithMany(item => item.InviteeNotifications);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.Notifications);

        builder
            .HasOne(item => item.Location)
            .WithMany(item => item.Notifications);

        builder
            .HasOne(item => item.Team)
            .WithMany(item => item.Notifications);

        builder.HasIndex(item => item.SourceId);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.EventRaisedAt);
    }
}
