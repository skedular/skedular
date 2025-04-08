using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamedForceContinuousSlotsToMustBookConsecutiveDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForceContinuousSlots",
                table: "ProductVersion",
                newName: "MustBookConsecutiveDays");

            migrationBuilder.RenameColumn(
                name: "ForceContinuousSlots",
                table: "Product",
                newName: "MustBookConsecutiveDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MustBookConsecutiveDays",
                table: "ProductVersion",
                newName: "ForceContinuousSlots");

            migrationBuilder.RenameColumn(
                name: "MustBookConsecutiveDays",
                table: "Product",
                newName: "ForceContinuousSlots");
        }
    }
}
