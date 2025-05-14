using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeConnectAccountAuthorization : EntityBase
{
    public bool IsAuthorized { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string StripeConnectAccountId { get; set; }
    public virtual StripeConnectAccount StripeConnectAccount { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeConnectAccountAuthorizationConfiguration : IEntityTypeConfiguration<StripeConnectAccountAuthorization>
{
    public void Configure(EntityTypeBuilder<StripeConnectAccountAuthorization> builder)
    {
        builder.ConfigureEntityBase();

        builder.HasOne(item => item.StripeConnectAccount)
            .WithOne(item => item.StripeConnectAccountAuthorization)
            .HasForeignKey<StripeConnectAccountAuthorization>(item => item.StripeConnectAccountId);

        builder.HasIndex(item => item.IsAuthorized);
    }
}
