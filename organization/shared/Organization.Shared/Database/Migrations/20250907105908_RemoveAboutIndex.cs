using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAboutIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_About",
                table: "Organization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Organization_About",
                table: "Organization",
                column: "About");
        }
    }
}
