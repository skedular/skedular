using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsListableFromOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_IsListable",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "IsListable",
                table: "Organization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsListable",
                table: "Organization",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_IsListable",
                table: "Organization",
                column: "IsListable");
        }
    }
}
