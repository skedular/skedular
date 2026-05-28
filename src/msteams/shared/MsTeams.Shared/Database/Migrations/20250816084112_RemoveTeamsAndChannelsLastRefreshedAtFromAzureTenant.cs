using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeamsAndChannelsLastRefreshedAtFromAzureTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_TeamsAndChannelsLastRefreshedAt",
                table: "AzureTenant");

            migrationBuilder.DropColumn(
                name: "TeamsAndChannelsLastRefreshedAt",
                table: "AzureTenant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TeamsAndChannelsLastRefreshedAt",
                table: "AzureTenant",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_TeamsAndChannelsLastRefreshedAt",
                table: "AzureTenant",
                column: "TeamsAndChannelsLastRefreshedAt");
        }
    }
}
