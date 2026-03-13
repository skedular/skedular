using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationPhysicalAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationPhysicalAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationPhysicalAddress",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    Coordinates = table.Column<Point>(type: "geometry (point, 4326)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationPhysicalAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationPhysicalAddress_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_Coordinates",
                table: "LocationPhysicalAddress",
                column: "Coordinates");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_CreatedAt",
                table: "LocationPhysicalAddress",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_LocationId",
                table: "LocationPhysicalAddress",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationPhysicalAddress_ModifiedAt",
                table: "LocationPhysicalAddress",
                column: "ModifiedAt");
        }
    }
}
