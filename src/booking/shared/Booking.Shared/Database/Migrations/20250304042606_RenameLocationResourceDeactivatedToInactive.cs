using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameLocationResourceDeactivatedToInactive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deactivated",
                table: "LocationResource",
                newName: "Inactive");

            migrationBuilder.RenameIndex(
                name: "IX_LocationResource_Deactivated",
                table: "LocationResource",
                newName: "IX_LocationResource_Inactive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Inactive",
                table: "LocationResource",
                newName: "Deactivated");

            migrationBuilder.RenameIndex(
                name: "IX_LocationResource_Inactive",
                table: "LocationResource",
                newName: "IX_LocationResource_Deactivated");
        }
    }
}
