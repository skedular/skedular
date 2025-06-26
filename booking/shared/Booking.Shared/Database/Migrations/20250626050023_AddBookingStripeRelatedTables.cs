using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStripeRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingCheckoutSession");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Organization",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Organization",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "Customer",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Customer",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Customer",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StripeCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StripeAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeCustomer_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StripeCustomer_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StripeProduct",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductVersionId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeProduct_ProductVersion_ProductVersionId",
                        column: x => x.ProductVersionId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StripeCheckoutSession",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeCheckoutSessionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CheckoutUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AmountTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    StripeCustomerCustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeCheckoutSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeCheckoutSession_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StripeCheckoutSession_StripeCustomer_StripeCustomerCustomer~",
                        column: x => x.StripeCustomerCustomerId,
                        principalTable: "StripeCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StripePrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripePriceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductVersionId = table.Column<string>(type: "character varying(100)", nullable: true),
                    StripeProductId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripePrice_ProductVersion_ProductVersionId",
                        column: x => x.ProductVersionId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StripePrice_StripeProduct_StripeProductId",
                        column: x => x.StripeProductId,
                        principalTable: "StripeProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_BookingId",
                table: "StripeCheckoutSession",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_CreatedAt",
                table: "StripeCheckoutSession",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_DeletedAt",
                table: "StripeCheckoutSession",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_ModifiedAt",
                table: "StripeCheckoutSession",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_PaymentStatus",
                table: "StripeCheckoutSession",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "StripeCheckoutSession",
                column: "StripeCheckoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_StripeCustomerCustomerId",
                table: "StripeCheckoutSession",
                column: "StripeCustomerCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_CreatedAt",
                table: "StripeCustomer",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_CustomerId",
                table: "StripeCustomer",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_DeletedAt",
                table: "StripeCustomer",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_ModifiedAt",
                table: "StripeCustomer",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_OrganizationId",
                table: "StripeCustomer",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_StripeAccountId",
                table: "StripeCustomer",
                column: "StripeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_StripeCustomerId",
                table: "StripeCustomer",
                column: "StripeCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_CreatedAt",
                table: "StripePrice",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_DeletedAt",
                table: "StripePrice",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_ModifiedAt",
                table: "StripePrice",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_ProductVersionId",
                table: "StripePrice",
                column: "ProductVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_StripeProductId",
                table: "StripePrice",
                column: "StripeProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_CreatedAt",
                table: "StripeProduct",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_DeletedAt",
                table: "StripeProduct",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_ModifiedAt",
                table: "StripeProduct",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_ProductVersionId",
                table: "StripeProduct",
                column: "ProductVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_StripeAccountId",
                table: "StripeProduct",
                column: "StripeAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeCheckoutSession");

            migrationBuilder.DropTable(
                name: "StripePrice");

            migrationBuilder.DropTable(
                name: "StripeCustomer");

            migrationBuilder.DropTable(
                name: "StripeProduct");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Locale",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Customer");

            migrationBuilder.CreateTable(
                name: "BookingCheckoutSession",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    AmountTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CheckoutUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCheckoutSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingCheckoutSession_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_BookingId",
                table: "BookingCheckoutSession",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_CreatedAt",
                table: "BookingCheckoutSession",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_DeletedAt",
                table: "BookingCheckoutSession",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_ModifiedAt",
                table: "BookingCheckoutSession",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_PaymentStatus",
                table: "BookingCheckoutSession",
                column: "PaymentStatus");
        }
    }
}
