using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentMethodAndIntent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StripeCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StripePaymentMethod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SetupIntentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentMethodId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CardBrand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardCountry = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CardDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CardExpiryMonth = table.Column<byte>(type: "smallint", nullable: true),
                    CardExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    CardFingerprint = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardFunding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CardIssuer = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardLastFourDigit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripePaymentMethod_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StripePaymentIntent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StripePaymentMethodId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePaymentIntent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripePaymentIntent_StripePaymentMethod_StripePaymentMethod~",
                        column: x => x.StripePaymentMethodId,
                        principalTable: "StripePaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_CreatedAt",
                table: "StripeCustomer",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_CustomerId",
                table: "StripeCustomer",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_DeletedAt",
                table: "StripeCustomer",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_ModifiedAt",
                table: "StripeCustomer",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_StripeCustomerId",
                table: "StripeCustomer",
                column: "StripeCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentIntent_Amount",
                table: "StripePaymentIntent",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentIntent_CreatedAt",
                table: "StripePaymentIntent",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentIntent_Currency",
                table: "StripePaymentIntent",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentIntent_DeletedAt",
                table: "StripePaymentIntent",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentIntent_ModifiedAt",
                table: "StripePaymentIntent",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentIntent_StripePaymentMethodId",
                table: "StripePaymentIntent",
                column: "StripePaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CardExpiryMonth_CardExpiryYear",
                table: "StripePaymentMethod",
                columns: new[] { "CardExpiryMonth", "CardExpiryYear" });

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CreatedAt",
                table: "StripePaymentMethod",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CustomerId",
                table: "StripePaymentMethod",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_DeletedAt",
                table: "StripePaymentMethod",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_ModifiedAt",
                table: "StripePaymentMethod",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_PaymentMethodId",
                table: "StripePaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_SetupIntentId",
                table: "StripePaymentMethod",
                column: "SetupIntentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeCustomer");

            migrationBuilder.DropTable(
                name: "StripePaymentIntent");

            migrationBuilder.DropTable(
                name: "StripePaymentMethod");
        }
    }
}
