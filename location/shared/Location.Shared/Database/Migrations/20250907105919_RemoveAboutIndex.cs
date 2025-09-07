using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAboutIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_About",
                table: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Location_About",
                table: "Location",
                column: "About");
        }
    }
}
