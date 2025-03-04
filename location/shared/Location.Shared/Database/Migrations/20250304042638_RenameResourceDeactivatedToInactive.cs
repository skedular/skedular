using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameResourceDeactivatedToInactive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deactivated",
                table: "Resource",
                newName: "Inactive");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_Deactivated",
                table: "Resource",
                newName: "IX_Resource_Inactive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Inactive",
                table: "Resource",
                newName: "Deactivated");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_Inactive",
                table: "Resource",
                newName: "IX_Resource_Deactivated");
        }
    }
}
