using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationOfferingDiscountPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "OrganizationOffering",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_DiscountPercentage",
                table: "OrganizationOffering",
                column: "DiscountPercentage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_DiscountPercentage",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "OrganizationOffering");
        }
    }
}
