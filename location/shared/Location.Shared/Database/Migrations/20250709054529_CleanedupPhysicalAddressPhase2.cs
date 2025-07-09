using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class CleanedupPhysicalAddressPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Address_AddressId",
                table: "Location");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Location_AddressId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressId",
                table: "Location",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Suburb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Zipcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_AddressId",
                table: "Location",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_CreatedAt",
                table: "Address",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Address_ModifiedAt",
                table: "Address",
                column: "ModifiedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Address_AddressId",
                table: "Location",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }
    }
}
