using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDailyMemberCountLastRecordedAtFromOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_DailyMemberCountLastRecordedAt",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "DailyMemberCountLastRecordedAt",
                table: "Organization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DailyMemberCountLastRecordedAt",
                table: "Organization",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_DailyMemberCountLastRecordedAt",
                table: "Organization",
                column: "DailyMemberCountLastRecordedAt");
        }
    }
}
