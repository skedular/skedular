using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slack.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Team_Timezone",
                table: "Team",
                column: "Timezone");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Timezone",
                table: "Location",
                column: "Timezone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Team_Timezone",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Location_Timezone",
                table: "Location");
        }
    }
}
