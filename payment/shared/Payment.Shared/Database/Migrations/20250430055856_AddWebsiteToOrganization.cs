using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripeConnectAccount_BusinessType",
                table: "StripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_StripeConnectAccount_Email",
                table: "StripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_StripeConnectAccount_Phone",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "StripeConnectAccount");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultCurrency",
                table: "StripeConnectAccount",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "StripeConnectAccount",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "StripeConnectAccount",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessType",
                table: "StripeConnectAccount",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "StripeConnectAccount",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "StripeConnectAccount",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportUrl",
                table: "StripeConnectAccount",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "StripeConnectAccount",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Organization",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "SupportUrl",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Organization");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultCurrency",
                table: "StripeConnectAccount",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "StripeConnectAccount",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "StripeConnectAccount",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessType",
                table: "StripeConnectAccount",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "StripeConnectAccount",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "StripeConnectAccount",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_BusinessType",
                table: "StripeConnectAccount",
                column: "BusinessType");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Email",
                table: "StripeConnectAccount",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Phone",
                table: "StripeConnectAccount",
                column: "Phone");
        }
    }
}
