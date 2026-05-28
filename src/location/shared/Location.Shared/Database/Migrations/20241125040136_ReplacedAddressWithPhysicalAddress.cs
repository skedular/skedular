using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedAddressWithPhysicalAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Address_AddressId",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Location",
                newName: "PhysicalAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_AddressId",
                table: "Location",
                newName: "IX_Location_PhysicalAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Address_PhysicalAddressId",
                table: "Location",
                column: "PhysicalAddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Address_PhysicalAddressId",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "PhysicalAddressId",
                table: "Location",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_PhysicalAddressId",
                table: "Location",
                newName: "IX_Location_AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Address_AddressId",
                table: "Location",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }
    }
}
