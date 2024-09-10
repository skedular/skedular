using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndLocationMembersConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocationMember_CustomerId",
                table: "LocationMember");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId_LocationId",
                table: "LocationMember",
                columns: new[] { "CustomerId", "LocationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocationMember_CustomerId_LocationId",
                table: "LocationMember");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId",
                table: "LocationMember",
                column: "CustomerId");
        }
    }
}
