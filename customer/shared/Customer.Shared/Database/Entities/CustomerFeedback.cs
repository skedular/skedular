using Api.Shared;
using Customer.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CustomerFeedback : EntityBase
{
    public string? Content { get; set; }
    public string Channel { get; set; }

    public virtual Customer Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CustomerFeedbackConfiguration : IEntityTypeConfiguration<CustomerFeedback>
{
    public void Configure(EntityTypeBuilder<CustomerFeedback> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Content).HasMaxLength(Constants.MaxFeedbackLength);
        builder.Property(item => item.Channel).HasMaxLength(Constants.MaxFeedbackChannelLength).HasDefaultValue(FeedbackChannelTypeConstants.Web);

        builder.HasOne(item => item.Customer).WithMany(item => item.CustomerFeedbacks);

        builder.HasIndex(item => item.Channel);
    }
}
