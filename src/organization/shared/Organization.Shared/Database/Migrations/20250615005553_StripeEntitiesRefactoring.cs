using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class StripeEntitiesRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_StripePaymentIntent_StripePaymentInten~",
                table: "OrganizationOffering");

            migrationBuilder.DropTable(
                name: "StripeCustomer");

            migrationBuilder.DropTable(
                name: "StripePaymentIntent");

            migrationBuilder.DropTable(
                name: "StripePaymentMethod");

            migrationBuilder.RenameColumn(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering",
                newName: "OrganizationStripePaymentIntentId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOffering_StripePaymentIntentId",
                table: "OrganizationOffering",
                newName: "IX_OrganizationOffering_OrganizationStripePaymentIntentId");

            migrationBuilder.CreateTable(
                name: "OrganizationStripeCustomer",
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
                    table.PrimaryKey("PK_OrganizationStripeCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripeCustomer_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStripePaymentMethod",
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
                    table.PrimaryKey("PK_OrganizationStripePaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStripePaymentIntent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OrganizationStripePaymentMethodId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStripePaymentIntent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripePaymentIntent_OrganizationStripePaymentMe~",
                        column: x => x.OrganizationStripePaymentMethodId,
                        principalTable: "OrganizationStripePaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeCustomer_CreatedAt",
                table: "OrganizationStripeCustomer",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeCustomer_DeletedAt",
                table: "OrganizationStripeCustomer",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeCustomer_ModifiedAt",
                table: "OrganizationStripeCustomer",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeCustomer_OrganizationId",
                table: "OrganizationStripeCustomer",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeCustomer_StripeCustomerId",
                table: "OrganizationStripeCustomer",
                column: "StripeCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentIntent_Amount",
                table: "OrganizationStripePaymentIntent",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentIntent_CreatedAt",
                table: "OrganizationStripePaymentIntent",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentIntent_Currency",
                table: "OrganizationStripePaymentIntent",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentIntent_DeletedAt",
                table: "OrganizationStripePaymentIntent",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentIntent_ModifiedAt",
                table: "OrganizationStripePaymentIntent",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentIntent_OrganizationStripePaymentMe~",
                table: "OrganizationStripePaymentIntent",
                column: "OrganizationStripePaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_CardExpiryMonth_CardExpiryY~",
                table: "OrganizationStripePaymentMethod",
                columns: new[] { "CardExpiryMonth", "CardExpiryYear" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_CreatedAt",
                table: "OrganizationStripePaymentMethod",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_DeletedAt",
                table: "OrganizationStripePaymentMethod",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_ModifiedAt",
                table: "OrganizationStripePaymentMethod",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_OrganizationId",
                table: "OrganizationStripePaymentMethod",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_PaymentMethodId",
                table: "OrganizationStripePaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_SetupIntentId",
                table: "OrganizationStripePaymentMethod",
                column: "SetupIntentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOffering_OrganizationStripePaymentIntent_Organi~",
                table: "OrganizationOffering",
                column: "OrganizationStripePaymentIntentId",
                principalTable: "OrganizationStripePaymentIntent",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_OrganizationStripePaymentIntent_Organi~",
                table: "OrganizationOffering");

            migrationBuilder.DropTable(
                name: "OrganizationStripeCustomer");

            migrationBuilder.DropTable(
                name: "OrganizationStripePaymentIntent");

            migrationBuilder.DropTable(
                name: "OrganizationStripePaymentMethod");

            migrationBuilder.RenameColumn(
                name: "OrganizationStripePaymentIntentId",
                table: "OrganizationOffering",
                newName: "StripePaymentIntentId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOffering_OrganizationStripePaymentIntentId",
                table: "OrganizationOffering",
                newName: "IX_OrganizationOffering_StripePaymentIntentId");

            migrationBuilder.CreateTable(
                name: "StripeCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
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
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CardBrand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardCountry = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CardDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CardExpiryMonth = table.Column<byte>(type: "smallint", nullable: true),
                    CardExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    CardFingerprint = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardFunding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CardIssuer = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CardLastFourDigit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentMethodId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SetupIntentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
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
                name: "StripePaymentIntent",
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
                name: "FK_OrganizationOffering_StripePaymentIntent_StripePaymentInten~",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                principalTable: "StripePaymentIntent",
                principalColumn: "Id");
        }
    }
}
