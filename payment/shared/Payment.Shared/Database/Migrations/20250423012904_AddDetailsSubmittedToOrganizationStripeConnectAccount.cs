using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailsSubmittedToOrganizationStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "PayoutsEnabled",
                table: "OrganizationStripeConnectAccount",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "ChargesEnabled",
                table: "OrganizationStripeConnectAccount",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<bool>(
                name: "DetailsSubmitted",
                table: "OrganizationStripeConnectAccount",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_DetailsSubmitted",
                table: "OrganizationStripeConnectAccount",
                column: "DetailsSubmitted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_DetailsSubmitted",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "DetailsSubmitted",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.AlterColumn<bool>(
                name: "PayoutsEnabled",
                table: "OrganizationStripeConnectAccount",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "ChargesEnabled",
                table: "OrganizationStripeConnectAccount",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
