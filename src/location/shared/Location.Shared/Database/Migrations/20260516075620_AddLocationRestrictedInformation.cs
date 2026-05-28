using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationRestrictedInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationRestrictedInformation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationRestrictedInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationRestrictedInformation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationRestrictedInformation_Active",
                table: "LocationRestrictedInformation",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRestrictedInformation_Category",
                table: "LocationRestrictedInformation",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRestrictedInformation_CreatedAt",
                table: "LocationRestrictedInformation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRestrictedInformation_LocationId",
                table: "LocationRestrictedInformation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRestrictedInformation_ModifiedAt",
                table: "LocationRestrictedInformation",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRestrictedInformation_SortOrder",
                table: "LocationRestrictedInformation",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationRestrictedInformation");
        }
    }
}
