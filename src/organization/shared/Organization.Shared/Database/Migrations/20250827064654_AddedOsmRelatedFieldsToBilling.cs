using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedOsmRelatedFieldsToBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Coordinates",
                table: "OrganizationBillingDetails",
                type: "geometry (point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "OrganizationBillingDetails",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmId",
                table: "OrganizationBillingDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmType",
                table: "OrganizationBillingDetails",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "OrganizationBillingDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBillingDetails_Coordinates",
                table: "OrganizationBillingDetails",
                column: "Coordinates");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBillingDetails_OsmId",
                table: "OrganizationBillingDetails",
                column: "OsmId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBillingDetails_OsmType",
                table: "OrganizationBillingDetails",
                column: "OsmType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBillingDetails_PlaceId",
                table: "OrganizationBillingDetails",
                column: "PlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationBillingDetails_Coordinates",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationBillingDetails_OsmId",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationBillingDetails_OsmType",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationBillingDetails_PlaceId",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropColumn(
                name: "OsmId",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropColumn(
                name: "OsmType",
                table: "OrganizationBillingDetails");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "OrganizationBillingDetails");
        }
    }
}
