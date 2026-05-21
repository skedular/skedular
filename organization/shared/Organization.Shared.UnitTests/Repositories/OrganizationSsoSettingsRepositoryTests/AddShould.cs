using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Shared.UnitTests.Repositories.OrganizationSsoSettingsRepositoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Fact]
    public void Track_Sso_Settings_As_New_Entity()
    {
        var dbContext = CreateDbContext();
        var sut = new OrganizationSsoSettingsRepository(dbContext, TimeProvider.System);
        var organizationSsoSettings = new OrganizationSsoSettings
        {
            Id = "sso-settings-1",
            OrganizationId = "organization-1",
            EntityId = "entity",
            LoginUrl = "https://login.example.com",
            AppFederationMetadataUrl = "https://login.example.com/metadata"
        };

        sut.Add(organizationSsoSettings);

        dbContext.Entry(organizationSsoSettings).State.ShouldBe(EntityState.Added);
    }

    private static OrganizationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OrganizationDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=local.test;Username=test;Password=test",
                builder => builder.UseNetTopologySuite())
            .Options;
        var customDbContextOptions = new CustomDbContextOptions<OrganizationDbContext> { IsPostgisEnabled = true };

        return new OrganizationDbContext(options, customDbContextOptions);
    }
}
