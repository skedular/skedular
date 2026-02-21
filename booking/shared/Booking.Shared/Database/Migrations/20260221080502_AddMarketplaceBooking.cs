using System;
using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Customer_PaidByCustomerId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Organization_PaidByOrganizationId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_StripeCheckoutSession_Booking_BookingId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropTable(
                name: "BookingProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Currency",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_IsPaymentRequired",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PaidByCustomerId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PaidByOrganizationId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PaymentMethod",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PaymentStatus",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TaxAmount",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TaxRatePercentage",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TotalAmount",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TotalAmountExcludeTax",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "InvoiceEmailList",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "IsPaymentRequired",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "LineItems",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaidByCustomerId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaidByOrganizationId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TaxRatePercentage",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TotalAmountExcludeTax",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "StripeCheckoutSession",
                newName: "MarketplaceBookingId");

            migrationBuilder.RenameIndex(
                name: "IX_StripeCheckoutSession_BookingId",
                table: "StripeCheckoutSession",
                newName: "IX_StripeCheckoutSession_MarketplaceBookingId");

            migrationBuilder.CreateTable(
                name: "MarketplaceBooking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "CONFIRMED"),
                    IsPaymentRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LineItems = table.Column<ICollection<ProductVersionLineItem>>(type: "jsonb", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PaymentExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalAmountExcludeTax = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TaxRatePercentage = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    InvoiceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InvoiceEmailList = table.Column<string>(type: "jsonb", nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PaidByCustomerId = table.Column<string>(type: "character varying(100)", nullable: true),
                    PaidByOrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBooking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceBooking_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBooking_Customer_PaidByCustomerId",
                        column: x => x.PaidByCustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketplaceBooking_Organization_PaidByOrganizationId",
                        column: x => x.PaidByOrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingProductVersion",
                columns: table => new
                {
                    MarketplaceBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingProductVersion", x => new { x.MarketplaceBookingsId, x.ProductVersionsId });
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingProductVersion_MarketplaceBooking_Marketp~",
                        column: x => x.MarketplaceBookingsId,
                        principalTable: "MarketplaceBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingProductVersion_ProductVersion_ProductVers~",
                        column: x => x.ProductVersionsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_BookingId",
                table: "MarketplaceBooking",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_CreatedAt",
                table: "MarketplaceBooking",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_Currency",
                table: "MarketplaceBooking",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_IsPaymentRequired",
                table: "MarketplaceBooking",
                column: "IsPaymentRequired");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_ModifiedAt",
                table: "MarketplaceBooking",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_PaidByCustomerId",
                table: "MarketplaceBooking",
                column: "PaidByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_PaidByOrganizationId",
                table: "MarketplaceBooking",
                column: "PaidByOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_PaymentExpiry",
                table: "MarketplaceBooking",
                column: "PaymentExpiry");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_PaymentMethod",
                table: "MarketplaceBooking",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_PaymentStatus",
                table: "MarketplaceBooking",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_TaxAmount",
                table: "MarketplaceBooking",
                column: "TaxAmount");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_TaxRatePercentage",
                table: "MarketplaceBooking",
                column: "TaxRatePercentage");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_TotalAmount",
                table: "MarketplaceBooking",
                column: "TotalAmount");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_TotalAmountExcludeTax",
                table: "MarketplaceBooking",
                column: "TotalAmountExcludeTax");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingProductVersion_ProductVersionsId",
                table: "MarketplaceBookingProductVersion",
                column: "ProductVersionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeCheckoutSession_MarketplaceBooking_MarketplaceBooking~",
                table: "StripeCheckoutSession",
                column: "MarketplaceBookingId",
                principalTable: "MarketplaceBooking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeCheckoutSession_MarketplaceBooking_MarketplaceBooking~",
                table: "StripeCheckoutSession");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingProductVersion");

            migrationBuilder.DropTable(
                name: "MarketplaceBooking");

            migrationBuilder.RenameColumn(
                name: "MarketplaceBookingId",
                table: "StripeCheckoutSession",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_StripeCheckoutSession_MarketplaceBookingId",
                table: "StripeCheckoutSession",
                newName: "IX_StripeCheckoutSession_BookingId");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Booking",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceEmailList",
                table: "Booking",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                table: "Booking",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentRequired",
                table: "Booking",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ICollection<ProductVersionLineItem>>(
                name: "LineItems",
                table: "Booking",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "PaidByCustomerId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaidByOrganizationId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "CONFIRMED");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRatePercentage",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountExcludeTax",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingProductVersion",
                columns: table => new
                {
                    BookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingProductVersion", x => new { x.BookingsId, x.ProductVersionsId });
                    table.ForeignKey(
                        name: "FK_BookingProductVersion_Booking_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingProductVersion_ProductVersion_ProductVersionsId",
                        column: x => x.ProductVersionsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Currency",
                table: "Booking",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_IsPaymentRequired",
                table: "Booking",
                column: "IsPaymentRequired");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaidByCustomerId",
                table: "Booking",
                column: "PaidByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaidByOrganizationId",
                table: "Booking",
                column: "PaidByOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaymentMethod",
                table: "Booking",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaymentStatus",
                table: "Booking",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TaxAmount",
                table: "Booking",
                column: "TaxAmount");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TaxRatePercentage",
                table: "Booking",
                column: "TaxRatePercentage");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TotalAmount",
                table: "Booking",
                column: "TotalAmount");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TotalAmountExcludeTax",
                table: "Booking",
                column: "TotalAmountExcludeTax");

            migrationBuilder.CreateIndex(
                name: "IX_BookingProductVersion_ProductVersionsId",
                table: "BookingProductVersion",
                column: "ProductVersionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Customer_PaidByCustomerId",
                table: "Booking",
                column: "PaidByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Organization_PaidByOrganizationId",
                table: "Booking",
                column: "PaidByOrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeCheckoutSession_Booking_BookingId",
                table: "StripeCheckoutSession",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
