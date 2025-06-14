using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPaymentMethodAndIntent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering");

            migrationBuilder.DropTable(
                name: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropTable(
                name: "StripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_StripePaymentIntentId",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StripePaymentMethod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    CardBrand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardCountry = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CardDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CardExpiryMonth = table.Column<byte>(type: "smallint", nullable: true),
                    CardExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    CardFingerprint = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardFunding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CardIssuer = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardLastFourDigit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ClientSecret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentMethodId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SetupIntentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripePaymentMethod_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StripePaymentMethod_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationOfferingStripePaymentIntent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripePaymentMethodId = table.Column<string>(type: "character varying(100)", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationOfferingStripePaymentIntent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                        column: x => x.StripePaymentMethodId,
                        principalTable: "StripePaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_StripePaymentIntentId",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Amount",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_CreatedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Currency",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_DeletedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_ModifiedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "StripePaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CardExpiryMonth_CardExpiryYear",
                table: "StripePaymentMethod",
                columns: new[] { "CardExpiryMonth", "CardExpiryYear" });

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_ClientSecret",
                table: "StripePaymentMethod",
                column: "ClientSecret");

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
                name: "IX_StripePaymentMethod_OrganizationId",
                table: "StripePaymentMethod",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_PaymentMethodId",
                table: "StripePaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_SetupIntentId",
                table: "StripePaymentMethod",
                column: "SetupIntentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_Status",
                table: "StripePaymentMethod",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                principalTable: "OrganizationOfferingStripePaymentIntent",
                principalColumn: "Id");
        }
    }
}
