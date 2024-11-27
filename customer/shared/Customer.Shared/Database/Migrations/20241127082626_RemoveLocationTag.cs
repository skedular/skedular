using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLocationTag");

            migrationBuilder.DropTable(
                name: "LocationTag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationTag_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLocationTag",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredLocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLocationTag", x => new { x.PreferredByCustomersId, x.PreferredLocationTagsId });
                    table.ForeignKey(
                        name: "FK_CustomerLocationTag_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerLocationTag_LocationTag_PreferredLocationTagsId",
                        column: x => x.PreferredLocationTagsId,
                        principalTable: "LocationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocationTag_PreferredLocationTagsId",
                table: "CustomerLocationTag",
                column: "PreferredLocationTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTag_DeletedAt",
                table: "LocationTag",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTag_LocationId",
                table: "LocationTag",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTag_Name",
                table: "LocationTag",
                column: "Name");
        }
    }
}
