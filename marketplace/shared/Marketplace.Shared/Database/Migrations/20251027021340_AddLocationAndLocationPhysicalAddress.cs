using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Marketplace.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAndLocationPhysicalAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LocationOrganizationTag",
                columns: table => new
                {
                    LocationsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationOrganizationTag", x => new { x.LocationsId, x.OrganizationTagsId });
                    table.ForeignKey(
                        name: "FK_LocationOrganizationTag_Location_LocationsId",
                        column: x => x.LocationsId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationOrganizationTag_OrganizationTag_OrganizationTagsId",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationPhysicalAddress",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Coordinates = table.Column<Point>(type: "geometry (point, 4326)", nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
                name: "IX_Location_CreatedAt",
                table: "Location",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DeletedAt",
                table: "Location",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_ModifiedAt",
                table: "Location",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_OrganizationId",
                table: "Location",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationOrganizationTag_OrganizationTagsId",
                table: "LocationOrganizationTag",
                column: "OrganizationTagsId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationOrganizationTag");

            migrationBuilder.DropTable(
                name: "LocationPhysicalAddress");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
