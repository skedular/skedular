using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDailyUpdateColumnsFromLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_DailyDeskCountLastRecordedAt",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_DailyRoomCountLastRecordedAt",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "DailyDeskCountLastRecordedAt",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "DailyRoomCountLastRecordedAt",
                table: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DailyDeskCountLastRecordedAt",
                table: "Location",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DailyRoomCountLastRecordedAt",
                table: "Location",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_DailyDeskCountLastRecordedAt",
                table: "Location",
                column: "DailyDeskCountLastRecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DailyRoomCountLastRecordedAt",
                table: "Location",
                column: "DailyRoomCountLastRecordedAt");
        }
    }
}
