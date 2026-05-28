using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationSsoSetting : ReplicatedEntityBase
{
    public string EntityId { get; set; }
    public string LoginUrl { get; set; }
    public string AppFederationMetadataUrl { get; set; }
    public bool IsActive { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationSsoConfiguration : IEntityTypeConfiguration<OrganizationSsoSetting>
{
    public void Configure(EntityTypeBuilder<OrganizationSsoSetting> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.EntityId).HasMaxLength(Api.Shared.Services.Constants.MaxSsoEntityIdLength);
        builder.Property(item => item.LoginUrl).HasMaxLength(Api.Shared.Services.Constants.MaxUrlLength);
        builder.Property(item => item.AppFederationMetadataUrl).HasMaxLength(Api.Shared.Services.Constants.MaxUrlLength);
        builder.Property(item => item.IsActive).HasDefaultValue(false);

        builder
            .HasOne(item => item.Organization)
            .WithOne(item => item.OrganizationSsoSettings)
            .HasForeignKey<OrganizationSsoSetting>(item => item.OrganizationId);
    }
}
