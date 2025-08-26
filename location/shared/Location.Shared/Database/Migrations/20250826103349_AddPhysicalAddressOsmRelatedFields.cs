using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalAddressOsmRelatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "LocationPhysicalAddress");

            migrationBuilder.AddColumn<Point>(
                name: "Coordinates",
                table: "LocationPhysicalAddress",
                type: "geometry(point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "LocationPhysicalAddress",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmId",
                table: "LocationPhysicalAddress",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmType",
                table: "LocationPhysicalAddress",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "LocationPhysicalAddress",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_Coordinates",
                table: "LocationPhysicalAddress",
                column: "Coordinates");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_OsmId",
                table: "LocationPhysicalAddress",
                column: "OsmId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_OsmType",
                table: "LocationPhysicalAddress",
                column: "OsmType");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_PlaceId",
                table: "LocationPhysicalAddress",
                column: "PlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocationPhysicalAddress_Coordinates",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropIndex(
                name: "IX_LocationPhysicalAddress_OsmId",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropIndex(
                name: "IX_LocationPhysicalAddress_OsmType",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropIndex(
                name: "IX_LocationPhysicalAddress_PlaceId",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "OsmId",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "OsmType",
                table: "LocationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "LocationPhysicalAddress");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "LocationPhysicalAddress",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "LocationPhysicalAddress",
                type: "numeric",
                nullable: true);
        }
    }
}
