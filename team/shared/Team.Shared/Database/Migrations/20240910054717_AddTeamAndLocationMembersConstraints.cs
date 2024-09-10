using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndLocationMembersConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_CustomerId",
                table: "TeamMember");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CustomerId_TeamId",
                table: "TeamMember",
                columns: new[] { "CustomerId", "TeamId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_CustomerId_TeamId",
                table: "TeamMember");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CustomerId",
                table: "TeamMember",
                column: "CustomerId");
        }
    }
}
