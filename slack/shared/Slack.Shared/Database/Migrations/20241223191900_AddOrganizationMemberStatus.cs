using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slack.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationMemberStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_Active",
                table: "OrganizationMember");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "OrganizationMember");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OrganizationMember",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "ACTIVE");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_Status",
                table: "OrganizationMember",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_Status",
                table: "OrganizationMember");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrganizationMember");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "OrganizationMember",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_Active",
                table: "OrganizationMember",
                column: "Active");
        }
    }
}
