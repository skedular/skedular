using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameResourceIsAvailableHoursOverridenToIsAvailableHoursOverridden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvailableHoursOverriden",
                table: "Resource",
                newName: "IsAvailableHoursOverridden");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_IsAvailableHoursOverriden",
                table: "Resource",
                newName: "IX_Resource_IsAvailableHoursOverridden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvailableHoursOverridden",
                table: "Resource",
                newName: "IsAvailableHoursOverriden");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_IsAvailableHoursOverridden",
                table: "Resource",
                newName: "IX_Resource_IsAvailableHoursOverriden");
        }
    }
}
