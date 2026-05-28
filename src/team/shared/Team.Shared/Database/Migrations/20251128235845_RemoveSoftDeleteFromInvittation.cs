using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeleteFromInvittation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_DeletedAt",
                table: "JoinInvitation");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JoinInvitation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "JoinInvitation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_DeletedAt",
                table: "JoinInvitation",
                column: "DeletedAt");
        }
    }
}
