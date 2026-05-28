using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamedProductTagsToOrganizationTagInProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId",
                table: "OrganizationTagProductVersion");

            migrationBuilder.RenameColumn(
                name: "ProductVersionProductTagId",
                table: "OrganizationTagProductVersion",
                newName: "ProductVersionOrganizationTagsId");

            migrationBuilder.RenameColumn(
                name: "ProductTagsId",
                table: "OrganizationTagProductVersion",
                newName: "OrganizationTagsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTagProductVersion_ProductVersionProductTagId",
                table: "OrganizationTagProductVersion",
                newName: "IX_OrganizationTagProductVersion_ProductVersionOrganizationTag~");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_OrganizationT~",
                table: "OrganizationTagProductVersion",
                column: "OrganizationTagsId",
                principalTable: "OrganizationTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_OrganizationT~",
                table: "OrganizationTagProductVersion");

            migrationBuilder.RenameColumn(
                name: "ProductVersionOrganizationTagsId",
                table: "OrganizationTagProductVersion",
                newName: "ProductVersionProductTagId");

            migrationBuilder.RenameColumn(
                name: "OrganizationTagsId",
                table: "OrganizationTagProductVersion",
                newName: "ProductTagsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTagProductVersion_ProductVersionOrganizationTag~",
                table: "OrganizationTagProductVersion",
                newName: "IX_OrganizationTagProductVersion_ProductVersionProductTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId",
                table: "OrganizationTagProductVersion",
                column: "ProductTagsId",
                principalTable: "OrganizationTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
