using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCustomerAndOrganizationBillingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingContactAddressLine1",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactAddressLine2",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactCity",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactCountry",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactEmail",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactProvince",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactSuburb",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingContactZipcode",
                table: "Organization");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingContactAddressLine1",
                table: "Organization",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactAddressLine2",
                table: "Organization",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactCity",
                table: "Organization",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactCountry",
                table: "Organization",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactEmail",
                table: "Organization",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactProvince",
                table: "Organization",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactSuburb",
                table: "Organization",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingContactZipcode",
                table: "Organization",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

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
        }
    }
}
