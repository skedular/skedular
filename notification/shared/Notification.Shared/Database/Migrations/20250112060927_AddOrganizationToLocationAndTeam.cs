using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationToLocationAndTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Team",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Location",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_OrganizationId",
                table: "Team",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_OrganizationId",
                table: "Location",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Organization_OrganizationId",
                table: "Team",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Organization_OrganizationId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_OrganizationId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Location_OrganizationId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Location");
        }
    }
}
