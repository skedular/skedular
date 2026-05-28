using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Identity : ReplicatedEntityBase, IIdentityDetails
{
    public virtual Customer Customer { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class IdentityConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.ConfigureReplicatedEntityBase(Constants.MaxVerifiableTokenLength);

        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);

        builder.HasOne(item => item.Customer).WithMany(item => item.Identities);

        builder.HasIndex(item => item.Email);
        builder.HasIndex(item => item.EmailVerified);
    }
}
