using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Location",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "PRIVATE");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Type",
                table: "Location",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_Type",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Location");
        }
    }
}
