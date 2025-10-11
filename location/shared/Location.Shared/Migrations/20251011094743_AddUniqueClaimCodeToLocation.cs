using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueClaimCodeToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueClaimCode",
                table: "Location",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_UniqueClaimCode",
                table: "Location",
                column: "UniqueClaimCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_UniqueClaimCode",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "UniqueClaimCode",
                table: "Location");
        }
    }
}
