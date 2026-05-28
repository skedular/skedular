using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedOsmRelatedFieldsToBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Coordinates",
                table: "CustomerBillingDetails",
                type: "geometry (point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "CustomerBillingDetails",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmId",
                table: "CustomerBillingDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmType",
                table: "CustomerBillingDetails",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "CustomerBillingDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBillingDetails_Coordinates",
                table: "CustomerBillingDetails",
                column: "Coordinates");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBillingDetails_OsmId",
                table: "CustomerBillingDetails",
                column: "OsmId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBillingDetails_OsmType",
                table: "CustomerBillingDetails",
                column: "OsmType");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBillingDetails_PlaceId",
                table: "CustomerBillingDetails",
                column: "PlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerBillingDetails_Coordinates",
                table: "CustomerBillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBillingDetails_OsmId",
                table: "CustomerBillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBillingDetails_OsmType",
                table: "CustomerBillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBillingDetails_PlaceId",
                table: "CustomerBillingDetails");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "CustomerBillingDetails");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "CustomerBillingDetails");

            migrationBuilder.DropColumn(
                name: "OsmId",
                table: "CustomerBillingDetails");

            migrationBuilder.DropColumn(
                name: "OsmType",
                table: "CustomerBillingDetails");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "CustomerBillingDetails");
        }
    }
}
