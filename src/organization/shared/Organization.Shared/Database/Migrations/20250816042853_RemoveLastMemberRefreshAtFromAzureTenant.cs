using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLastMemberRefreshAtFromAzureTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_MembersLastRefreshedAt",
                table: "AzureTenant");

            migrationBuilder.DropColumn(
                name: "MembersLastRefreshedAt",
                table: "AzureTenant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MembersLastRefreshedAt",
                table: "AzureTenant",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_MembersLastRefreshedAt",
                table: "AzureTenant",
                column: "MembersLastRefreshedAt");
        }
    }
}
