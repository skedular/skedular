using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUnusedReplicatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Team_Name",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Resource_Name",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_Name",
                table: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_Location_Name",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "OrganizationTag");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrganizationTag");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Team",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Resource",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Resource",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "OrganizationTag",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrganizationTag",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Location",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_Name",
                table: "Team",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Name",
                table: "Resource",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_Name",
                table: "OrganizationTag",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Name",
                table: "Location",
                column: "Name");
        }
    }
}
