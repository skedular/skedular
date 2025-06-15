using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFloorPlanAndResourcePosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FloorPlan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FloorLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FloorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ThumbnailPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FloorPlan_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourcePosition",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Shape = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Metadata = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: true),
                    ResourceId = table.Column<string>(type: "character varying(100)", nullable: false),
                    FloorPlanId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourcePosition_FloorPlan_FloorPlanId",
                        column: x => x.FloorPlanId,
                        principalTable: "FloorPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourcePosition_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_CreatedAt",
                table: "FloorPlan",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_DeletedAt",
                table: "FloorPlan",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_FloorLevel",
                table: "FloorPlan",
                column: "FloorLevel");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_IsActive",
                table: "FloorPlan",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_LocationId",
                table: "FloorPlan",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan",
                columns: new[] { "LocationId", "FloorLevel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_ModifiedAt",
                table: "FloorPlan",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_Name",
                table: "FloorPlan",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePosition_CreatedAt",
                table: "ResourcePosition",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePosition_DeletedAt",
                table: "ResourcePosition",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePosition_FloorPlanId",
                table: "ResourcePosition",
                column: "FloorPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePosition_ModifiedAt",
                table: "ResourcePosition",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePosition_ResourceId",
                table: "ResourcePosition",
                column: "ResourceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourcePosition");

            migrationBuilder.DropTable(
                name: "FloorPlan");
        }
    }
}
