using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOnboardingDoneToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultOrganizationOnboardingDone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsLocationOnboardingDone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsOrganizationOnboardingDone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsPreferredLocationOnboardingDone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsPreferredZoneOnboardingDone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsTeamOnboardingDone",
                table: "Customer");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_IsOnboardingDone",
                table: "Customer",
                column: "IsOnboardingDone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customer_IsOnboardingDone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsOnboardingDone",
                table: "Customer");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultOrganizationOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocationOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOrganizationOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreferredLocationOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreferredZoneOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeamOnboardingDone",
                table: "Customer",
                type: "boolean",
                nullable: true);
        }
    }
}
