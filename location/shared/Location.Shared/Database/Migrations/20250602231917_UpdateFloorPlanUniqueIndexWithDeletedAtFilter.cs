using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFloorPlanUniqueIndexWithDeletedAtFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan",
                columns: new[] { "LocationId", "FloorLevel" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan",
                columns: new[] { "LocationId", "FloorLevel" },
                unique: true);
        }
    }
}
