using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameResourceIsOpeningHoursOverridenToIsAvailableHoursOverriden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOpeningHoursOverriden",
                table: "Resource",
                newName: "IsAvailableHoursOverriden");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_IsOpeningHoursOverriden",
                table: "Resource",
                newName: "IX_Resource_IsAvailableHoursOverriden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvailableHoursOverriden",
                table: "Resource",
                newName: "IsOpeningHoursOverriden");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_IsAvailableHoursOverriden",
                table: "Resource",
                newName: "IX_Resource_IsOpeningHoursOverriden");
        }
    }
}
