using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_Code",
                table: "OrganizationOffering",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_End",
                table: "OrganizationOffering",
                column: "End");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_Start",
                table: "OrganizationOffering",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization",
                column: "StripeCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_Code",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_End",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_Start",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization");
        }
    }
}
