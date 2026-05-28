using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationArrearsInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationArrearsInvoice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InvoiceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    BillingPeriodStartInclusive = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BillingPeriodEndExclusive = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationArrearsInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationArrearsInvoice_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationArrearsInvoice_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationArrearsInvoiceLine",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SegmentKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ServicePeriodStartInclusive = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ServicePeriodEndExclusive = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EarnedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Description = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationArrearsInvoiceId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationArrearsInvoiceLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationArrearsInvoiceLine_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationArrearsInvoiceLine_OrganizationArrearsInvoice_O~",
                        column: x => x.OrganizationArrearsInvoiceId,
                        principalTable: "OrganizationArrearsInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_BillingPeriodEndExclusive",
                table: "OrganizationArrearsInvoice",
                column: "BillingPeriodEndExclusive");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_BillingPeriodStartInclusive",
                table: "OrganizationArrearsInvoice",
                column: "BillingPeriodStartInclusive");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_CreatedAt",
                table: "OrganizationArrearsInvoice",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_Currency",
                table: "OrganizationArrearsInvoice",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_CustomerId",
                table: "OrganizationArrearsInvoice",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_ModifiedAt",
                table: "OrganizationArrearsInvoice",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_OrganizationId",
                table: "OrganizationArrearsInvoice",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoice_OrganizationId_InvoiceNumber",
                table: "OrganizationArrearsInvoice",
                columns: new[] { "OrganizationId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_Amount",
                table: "OrganizationArrearsInvoiceLine",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_BookingId",
                table: "OrganizationArrearsInvoiceLine",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_CreatedAt",
                table: "OrganizationArrearsInvoiceLine",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_EarnedAt",
                table: "OrganizationArrearsInvoiceLine",
                column: "EarnedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_ModifiedAt",
                table: "OrganizationArrearsInvoiceLine",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_OrganizationArrearsInvoiceId",
                table: "OrganizationArrearsInvoiceLine",
                column: "OrganizationArrearsInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationArrearsInvoiceLine_SegmentKey",
                table: "OrganizationArrearsInvoiceLine",
                column: "SegmentKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationArrearsInvoiceLine");

            migrationBuilder.DropTable(
                name: "OrganizationArrearsInvoice");
        }
    }
}
