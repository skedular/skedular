using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationSsoSetting : EntityBase
{
    public string EntityId { get; set; }
    public string LoginUrl { get; set; }
    public string AppFederationMetadataUrl { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationSsoConfiguration : IEntityTypeConfiguration<OrganizationSsoSetting>
{
    public void Configure(EntityTypeBuilder<OrganizationSsoSetting> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.EntityId).HasMaxLength(Constants.MaxSsoEntityIdLength);
        builder.Property(item => item.LoginUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.AppFederationMetadataUrl).HasMaxLength(Constants.MaxUrlLength);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.OrganizationSsoSettings);
    }
}
