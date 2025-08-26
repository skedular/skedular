using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalAddressOsmRelatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.AddColumn<Point>(
                name: "Coordinates",
                table: "OrganizationPhysicalAddress",
                type: "geometry (point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "OrganizationPhysicalAddress",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmId",
                table: "OrganizationPhysicalAddress",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsmType",
                table: "OrganizationPhysicalAddress",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "OrganizationPhysicalAddress",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "OsmId",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "OsmType",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "OrganizationPhysicalAddress",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "OrganizationPhysicalAddress",
                type: "numeric",
                nullable: true);
        }
    }
}
