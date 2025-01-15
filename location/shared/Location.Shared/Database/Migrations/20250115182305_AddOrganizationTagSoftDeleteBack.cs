using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTagSoftDeleteBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "OrganizationTag",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_DeletedAt",
                table: "OrganizationTag",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_DeletedAt",
                table: "OrganizationTag");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OrganizationTag");
        }
    }
}
