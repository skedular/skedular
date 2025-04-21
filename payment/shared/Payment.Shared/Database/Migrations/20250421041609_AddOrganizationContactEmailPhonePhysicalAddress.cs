using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationContactEmailPhonePhysicalAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Organization",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Organization",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

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
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Suburb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Zipcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                name: "ContactEmail",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "PhysicalAddressId",
                table: "Organization");
        }
    }
}
