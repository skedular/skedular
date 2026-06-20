using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpacesOfferingCatalogVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 UPDATE "OrganizationOffering"
                                 SET "CatalogVersion" = 'SPACES_V1'
                                 WHERE "Code" IN (40000, 50000, 60000, 70000)
                                   AND ("CatalogVersion" IS NULL OR "CatalogVersion" = 'TEAMS_V1');
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 UPDATE "OrganizationOffering"
                                 SET "CatalogVersion" = 'TEAMS_V1'
                                 WHERE "Code" IN (40000, 50000, 60000, 70000)
                                   AND "CatalogVersion" = 'SPACES_V1';
                                 """);
        }
    }
}
