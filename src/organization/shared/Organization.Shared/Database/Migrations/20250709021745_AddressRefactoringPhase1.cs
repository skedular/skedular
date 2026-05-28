using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddressRefactoringPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organization_Address_PhysicalAddressId",
                table: "Organization");

            migrationBuilder.RenameColumn(
                name: "PhysicalAddressId",
                table: "Organization",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_PhysicalAddressId",
                table: "Organization",
                newName: "IX_Organization_AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Address_AddressId",
                table: "Organization",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organization_Address_AddressId",
                table: "Organization");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Organization",
                newName: "PhysicalAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_AddressId",
                table: "Organization",
                newName: "IX_Organization_PhysicalAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Address_PhysicalAddressId",
                table: "Organization",
                column: "PhysicalAddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }
    }
}
