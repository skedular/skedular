using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyResourceAvailabilitySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyResourceAvailabilitySnapshot",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Classification = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ResourceId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyResourceAvailabilitySnapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyResourceAvailabilitySnapshot_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyResourceAvailabilitySnapshot_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_Classification",
                table: "DailyResourceAvailabilitySnapshot",
                column: "Classification");

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_CreatedAt",
                table: "DailyResourceAvailabilitySnapshot",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_Date",
                table: "DailyResourceAvailabilitySnapshot",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_DeletedAt",
                table: "DailyResourceAvailabilitySnapshot",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_LocationId_Date",
                table: "DailyResourceAvailabilitySnapshot",
                columns: new[] { "LocationId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_ModifiedAt",
                table: "DailyResourceAvailabilitySnapshot",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyResourceAvailabilitySnapshot_ResourceId",
                table: "DailyResourceAvailabilitySnapshot",
                column: "ResourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyResourceAvailabilitySnapshot");
        }
    }
}
