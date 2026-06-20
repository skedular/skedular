using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class NullUnlimitedOrganizationOfferingCapacities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 UPDATE "OrganizationOffering"
                                 SET
                                     "PurchasedUserCapacity" = NULLIF("PurchasedUserCapacity", -1),
                                     "PurchasedLocationCapacity" = NULLIF("PurchasedLocationCapacity", -1),
                                     "PurchasedTeamCapacity" = NULLIF("PurchasedTeamCapacity", -1)
                                 WHERE "PurchasedUserCapacity" = -1
                                    OR "PurchasedLocationCapacity" = -1
                                    OR "PurchasedTeamCapacity" = -1;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 UPDATE "OrganizationOffering"
                                 SET
                                     "PurchasedUserCapacity" = CASE
                                         WHEN "PurchasedUserCapacity" IS NULL
                                             AND "Code" IN (0, 20000, 30000, 40000, 50000, 60000, 70000, 1000000)
                                             THEN -1
                                         ELSE "PurchasedUserCapacity"
                                     END,
                                     "PurchasedLocationCapacity" = CASE
                                         WHEN "PurchasedLocationCapacity" IS NULL
                                             AND "Code" IN (0, 20000, 30000, 40000, 50000, 60000, 70000, 1000000)
                                             THEN -1
                                         ELSE "PurchasedLocationCapacity"
                                     END,
                                     "PurchasedTeamCapacity" = CASE
                                         WHEN "PurchasedTeamCapacity" IS NULL
                                             AND "Code" IN (0, 20000, 30000, 70000, 1000000)
                                             THEN -1
                                         ELSE "PurchasedTeamCapacity"
                                     END
                                 WHERE "PurchasedUserCapacity" IS NULL
                                    OR "PurchasedLocationCapacity" IS NULL
                                    OR "PurchasedTeamCapacity" IS NULL;
                                 """);
        }
    }
}
