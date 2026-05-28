using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddContactedViaFieldsToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ContactedViaCall",
                table: "Location",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContactedViaEmail",
                table: "Location",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContactedViaSms",
                table: "Location",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContactedViaWhatsapp",
                table: "Location",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Location_ContactedViaCall",
                table: "Location",
                column: "ContactedViaCall");

            migrationBuilder.CreateIndex(
                name: "IX_Location_ContactedViaEmail",
                table: "Location",
                column: "ContactedViaEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Location_ContactedViaSms",
                table: "Location",
                column: "ContactedViaSms");

            migrationBuilder.CreateIndex(
                name: "IX_Location_ContactedViaWhatsapp",
                table: "Location",
                column: "ContactedViaWhatsapp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_ContactedViaCall",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_ContactedViaEmail",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_ContactedViaSms",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_ContactedViaWhatsapp",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ContactedViaCall",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ContactedViaEmail",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ContactedViaSms",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ContactedViaWhatsapp",
                table: "Location");
        }
    }
}
