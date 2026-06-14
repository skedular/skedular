using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingCatalogFieldsToOrganizationOffering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CatalogVersion",
                table: "OrganizationOffering",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "OrganizationOffering",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "usd");

            migrationBuilder.AddColumn<int>(
                name: "FixedPrice",
                table: "OrganizationOffering",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedLocationCapacity",
                table: "OrganizationOffering",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedTeamCapacity",
                table: "OrganizationOffering",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedUserCapacity",
                table: "OrganizationOffering",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitPrice",
                table: "OrganizationOffering",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.Sql("""
                                 UPDATE "OrganizationOffering"
                                 SET
                                     "PurchasedUserCapacity" = CASE
                                         WHEN "Code" = 10000 THEN 10
                                         WHEN "Code" = 20000 THEN -1
                                         WHEN "Code" = 30000 THEN -1
                                         WHEN "Code" = 1000000 THEN -1
                                         ELSE "PurchasedUserCapacity"
                                     END,
                                     "FixedPrice" = CASE
                                         WHEN "Code" = 20000 THEN NULL
                                         WHEN "Code" = 30000 AND "UnitPrice" > 0 THEN "UnitPrice"
                                         WHEN "Code" = 1000000 AND "UnitPrice" > 0 THEN "UnitPrice"
                                         ELSE 0
                                     END,
                                     "PurchasedLocationCapacity" = CASE
                                         WHEN "Code" = 10000 THEN 1
                                         WHEN "Code" = 20000 THEN -1
                                         WHEN "Code" = 30000 THEN -1
                                         WHEN "Code" = 1000000 THEN -1
                                         ELSE "PurchasedLocationCapacity"
                                     END,
                                     "PurchasedTeamCapacity" = CASE
                                         WHEN "Code" = 10000 THEN 1
                                         WHEN "Code" = 20000 THEN -1
                                         WHEN "Code" = 30000 THEN -1
                                         WHEN "Code" = 1000000 THEN -1
                                         ELSE "PurchasedTeamCapacity"
                                     END,
                                     "UnitPrice" = CASE
                                         WHEN "Code" = 20000 THEN "UnitPrice"
                                         ELSE NULL
                                     END,
                                     "CatalogVersion" = COALESCE("CatalogVersion", 'TEAMS_V1'),
                                     "Currency" = COALESCE("Currency", 'usd');
                                 """);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_CatalogVersion",
                table: "OrganizationOffering",
                column: "CatalogVersion");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_Currency",
                table: "OrganizationOffering",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_FixedPrice",
                table: "OrganizationOffering",
                column: "FixedPrice");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_PurchasedLocationCapacity",
                table: "OrganizationOffering",
                column: "PurchasedLocationCapacity");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_PurchasedTeamCapacity",
                table: "OrganizationOffering",
                column: "PurchasedTeamCapacity");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_PurchasedUserCapacity",
                table: "OrganizationOffering",
                column: "PurchasedUserCapacity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_CatalogVersion",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_Currency",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_FixedPrice",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_PurchasedLocationCapacity",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_PurchasedTeamCapacity",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_PurchasedUserCapacity",
                table: "OrganizationOffering");

            migrationBuilder.Sql("""
                                 UPDATE "OrganizationOffering"
                                 SET "UnitPrice" = 0
                                 WHERE "UnitPrice" IS NULL;
                                 """);

            migrationBuilder.AlterColumn<int>(
                name: "UnitPrice",
                table: "OrganizationOffering",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "CatalogVersion",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "FixedPrice",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "PurchasedLocationCapacity",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "PurchasedTeamCapacity",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "PurchasedUserCapacity",
                table: "OrganizationOffering");
        }
    }
}
