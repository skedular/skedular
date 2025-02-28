using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
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

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Timezone",
                table: "Customer",
                column: "Timezone");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeamChannel_Email",
                table: "AzureTenantTeamChannel",
                column: "Email");
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

            migrationBuilder.DropIndex(
                name: "IX_Customer_Timezone",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantTeamChannel_Email",
                table: "AzureTenantTeamChannel");
        }
    }
}
