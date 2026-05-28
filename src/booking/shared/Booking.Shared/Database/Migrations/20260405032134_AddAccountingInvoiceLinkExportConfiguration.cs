using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingInvoiceLinkExportConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExportConfigurationMessage",
                table: "AccountingInvoiceLink",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExportConfigurationState",
                table: "AccountingInvoiceLink",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalInvoiceMode",
                table: "AccountingInvoiceLink",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RepeatingSchedulePeriod",
                table: "AccountingInvoiceLink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepeatingScheduleSource",
                table: "AccountingInvoiceLink",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepeatingScheduleUnit",
                table: "AccountingInvoiceLink",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportConfigurationMessage",
                table: "AccountingInvoiceLink");

            migrationBuilder.DropColumn(
                name: "ExportConfigurationState",
                table: "AccountingInvoiceLink");

            migrationBuilder.DropColumn(
                name: "ExternalInvoiceMode",
                table: "AccountingInvoiceLink");

            migrationBuilder.DropColumn(
                name: "RepeatingSchedulePeriod",
                table: "AccountingInvoiceLink");

            migrationBuilder.DropColumn(
                name: "RepeatingScheduleSource",
                table: "AccountingInvoiceLink");

            migrationBuilder.DropColumn(
                name: "RepeatingScheduleUnit",
                table: "AccountingInvoiceLink");
        }
    }
}
