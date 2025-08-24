using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissingOsmIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhysicalAddress_Coordinates",
                table: "OrganizationPhysicalAddress",
                column: "Coordinates");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhysicalAddress_OsmId",
                table: "OrganizationPhysicalAddress",
                column: "OsmId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhysicalAddress_OsmType",
                table: "OrganizationPhysicalAddress",
                column: "OsmType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhysicalAddress_PlaceId",
                table: "OrganizationPhysicalAddress",
                column: "PlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationPhysicalAddress_Coordinates",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPhysicalAddress_OsmId",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPhysicalAddress_OsmType",
                table: "OrganizationPhysicalAddress");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPhysicalAddress_PlaceId",
                table: "OrganizationPhysicalAddress");
        }
    }
}
