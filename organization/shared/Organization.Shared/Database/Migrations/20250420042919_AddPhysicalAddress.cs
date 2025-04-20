using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhysicalAddressId",
                table: "Organization",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FormattedAddress = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Suburb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Zipcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_PhysicalAddressId",
                table: "Organization",
                column: "PhysicalAddressId",
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
                name: "FK_Organization_Address_PhysicalAddressId",
                table: "Organization",
                column: "PhysicalAddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organization_Address_PhysicalAddressId",
                table: "Organization");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Organization_PhysicalAddressId",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "PhysicalAddressId",
                table: "Organization");
        }
    }
}
