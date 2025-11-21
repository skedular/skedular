using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOwnershipVerifiedToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOwnershipVerified",
                table: "Organization",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_IsOwnershipVerified",
                table: "Organization",
                column: "IsOwnershipVerified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_IsOwnershipVerified",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "IsOwnershipVerified",
                table: "Organization");
        }
    }
}
