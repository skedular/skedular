using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeStripeModelReusable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_OrganizationOfferin~",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_OrganizationStripeP~",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_Organization_Organi~",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationStripePaymentMethod",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_CardExpiryMonth_CardExpiryY~",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_ClientSecret",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_CreatedAt",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_DeletedAt",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_ModifiedAt",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_PaymentMethodId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_SetupIntentId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripePaymentMethod_Status",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationId",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationOfferin~",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardBrand",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardCountry",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardDescription",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardExpiryMonth",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardExpiryYear",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardFingerprint",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardFunding",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardIssuer",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CardLastFourDigit",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "SetupIntentId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropColumn(
                name: "OrganizationOfferingId",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "OrganizationStripePaymentMethod",
                newName: "StripePaymentMethodsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStripePaymentMethod_OrganizationId",
                table: "OrganizationStripePaymentMethod",
                newName: "IX_OrganizationStripePaymentMethod_StripePaymentMethodsId");

            migrationBuilder.RenameColumn(
                name: "OrganizationStripePaymentMethodId",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "StripePaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationStripeP~",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationsId",
                table: "OrganizationStripePaymentMethod",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationStripePaymentMethod",
                table: "OrganizationStripePaymentMethod",
                columns: new[] { "OrganizationsId", "StripePaymentMethodsId" });

            migrationBuilder.CreateTable(
                name: "StripePaymentMethod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SetupIntentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
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
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Amount",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Currency",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_StripePaymentIntentId",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "StripePaymentMethodId",
                principalTable: "StripePaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationsId",
                table: "OrganizationStripePaymentMethod",
                column: "OrganizationsId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStripePaymentMethod_StripePaymentMethod_StripeP~",
                table: "OrganizationStripePaymentMethod",
                column: "StripePaymentMethodsId",
                principalTable: "StripePaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationsId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStripePaymentMethod_StripePaymentMethod_StripeP~",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropTable(
                name: "StripePaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationStripePaymentMethod",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Amount",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Currency",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_StripePaymentIntentId",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "OrganizationsId",
                table: "OrganizationStripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "OrganizationOffering");

            migrationBuilder.RenameColumn(
                name: "StripePaymentMethodsId",
                table: "OrganizationStripePaymentMethod",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStripePaymentMethod_StripePaymentMethodsId",
                table: "OrganizationStripePaymentMethod",
                newName: "IX_OrganizationStripePaymentMethod_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "StripePaymentMethodId",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "OrganizationStripePaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_OrganizationStripeP~");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "OrganizationStripePaymentMethod",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardBrand",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardCountry",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardDescription",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CardExpiryMonth",
                table: "OrganizationStripePaymentMethod",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CardExpiryYear",
                table: "OrganizationStripePaymentMethod",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardFingerprint",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardFunding",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardIssuer",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardLastFourDigit",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "OrganizationStripePaymentMethod",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "OrganizationStripePaymentMethod",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedAt",
                table: "OrganizationStripePaymentMethod",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetupIntentId",
                table: "OrganizationStripePaymentMethod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrganizationStripePaymentMethod",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "OrganizationStripePaymentMethod",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "OrganizationOfferingStripePaymentIntent",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationOfferingId",
                table: "OrganizationOfferingStripePaymentIntent",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationStripePaymentMethod",
                table: "OrganizationStripePaymentMethod",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_CardExpiryMonth_CardExpiryY~",
                table: "OrganizationStripePaymentMethod",
                columns: new[] { "CardExpiryMonth", "CardExpiryYear" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_ClientSecret",
                table: "OrganizationStripePaymentMethod",
                column: "ClientSecret");

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
                name: "IX_OrganizationStripePaymentMethod_PaymentMethodId",
                table: "OrganizationStripePaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_SetupIntentId",
                table: "OrganizationStripePaymentMethod",
                column: "SetupIntentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_Status",
                table: "OrganizationStripePaymentMethod",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationId",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationOfferin~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationOfferingId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_OrganizationOfferin~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationOfferingId",
                principalTable: "OrganizationOffering",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_OrganizationStripeP~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationStripePaymentMethodId",
                principalTable: "OrganizationStripePaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_Organization_Organi~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationId",
                table: "OrganizationStripePaymentMethod",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
