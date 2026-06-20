using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddHostCommissionToOrganizationOffering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HostCommissionPercentage",
                table: "OrganizationOffering",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 5m);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_HostCommissionPercentage",
                table: "OrganizationOffering",
                column: "HostCommissionPercentage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_HostCommissionPercentage",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "HostCommissionPercentage",
                table: "OrganizationOffering");

        }
    }
}
