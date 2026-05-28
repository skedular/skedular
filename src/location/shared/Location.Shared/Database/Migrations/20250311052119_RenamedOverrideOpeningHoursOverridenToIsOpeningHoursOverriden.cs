using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamedOverrideOpeningHoursOverridenToIsOpeningHoursOverriden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OverrideOpeningHoursOverriden",
                table: "Resource",
                newName: "IsOpeningHoursOverriden");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_OverrideOpeningHoursOverriden",
                table: "Resource",
                newName: "IX_Resource_IsOpeningHoursOverriden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOpeningHoursOverriden",
                table: "Resource",
                newName: "OverrideOpeningHoursOverriden");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_IsOpeningHoursOverriden",
                table: "Resource",
                newName: "IX_Resource_OverrideOpeningHoursOverriden");
        }
    }
}
