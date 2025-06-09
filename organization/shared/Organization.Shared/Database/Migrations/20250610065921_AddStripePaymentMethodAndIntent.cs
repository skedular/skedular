using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentMethodAndIntent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_HasAttachedPaymentMethod",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "HasAttachedPaymentMethod",
                table: "Organization");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StripeCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeCustomer_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
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
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripePaymentMethod_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationOfferingStripePaymentIntent",
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
                name: "IX_StripeCustomer_CreatedAt",
                table: "StripeCustomer",
                column: "CreatedAt");

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
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_StripeCustomerId",
                table: "StripeCustomer",
                column: "StripeCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CardExpiryMonth_CardExpiryYear",
                table: "StripePaymentMethod",
                columns: new[] { "CardExpiryMonth", "CardExpiryYear" });

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CreatedAt",
                table: "StripePaymentMethod",
                column: "CreatedAt");

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

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                principalTable: "OrganizationOfferingStripePaymentIntent",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering");

            migrationBuilder.DropTable(
                name: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropTable(
                name: "StripeCustomer");

            migrationBuilder.DropTable(
                name: "StripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_StripePaymentIntentId",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering");

            migrationBuilder.AddColumn<bool>(
                name: "HasAttachedPaymentMethod",
                table: "Organization",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_HasAttachedPaymentMethod",
                table: "Organization",
                column: "HasAttachedPaymentMethod");
        }
    }
}
