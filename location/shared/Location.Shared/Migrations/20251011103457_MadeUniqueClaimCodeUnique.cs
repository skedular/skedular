using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class MadeUniqueClaimCodeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_UniqueClaimCode",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Location_UniqueClaimCode",
                table: "Location",
                column: "UniqueClaimCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_UniqueClaimCode",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Location_UniqueClaimCode",
                table: "Location",
                column: "UniqueClaimCode");
        }
    }
}
