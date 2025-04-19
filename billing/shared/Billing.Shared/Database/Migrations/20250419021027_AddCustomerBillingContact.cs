using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerBillingContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingContactAddressLine1",
                table: "Customer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactAddressLine2",
                table: "Customer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactCity",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactCompanyName",
                table: "Customer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactCountry",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactEmail",
                table: "Customer",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactProvince",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactSuburb",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactZipcode",
                table: "Customer",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Customer",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingContactAddressLine1",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactAddressLine2",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactCity",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactCompanyName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactCountry",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactEmail",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactProvince",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactSuburb",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BillingContactZipcode",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Customer");
        }
    }
}
