using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slack.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAllRefreshRelatedColumnsFromWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workspace_ChannelsLastRefreshedAt",
                table: "Workspace");

            migrationBuilder.DropIndex(
                name: "IX_Workspace_MembersLastRefreshedAt",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "ChannelsLastRefreshedAt",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "LastRefreshedAt",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "MembersLastRefreshedAt",
                table: "Workspace");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChannelsLastRefreshedAt",
                table: "Workspace",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastRefreshedAt",
                table: "Workspace",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MembersLastRefreshedAt",
                table: "Workspace",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_ChannelsLastRefreshedAt",
                table: "Workspace",
                column: "ChannelsLastRefreshedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_MembersLastRefreshedAt",
                table: "Workspace",
                column: "MembersLastRefreshedAt");
        }
    }
}
