using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMemberStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TeamMember",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "ACTIVE");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_Status",
                table: "TeamMember",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_Status",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TeamMember");
        }
    }
}
