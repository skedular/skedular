using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class PreparingStripeCustomerMigrationPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripeCustomer_DeletedAt",
                table: "StripeCustomer");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StripeCustomer");

            migrationBuilder.AlterColumn<string>(
                name: "StripeCustomerId",
                table: "StripeCustomer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Organization",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Customer",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization",
                column: "StripeCustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_StripeCustomerId",
                table: "Customer",
                column: "StripeCustomerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_StripeCustomer_StripeCustomerId",
                table: "Customer",
                column: "StripeCustomerId",
                principalTable: "StripeCustomer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_StripeCustomer_StripeCustomerId",
                table: "Organization",
                column: "StripeCustomerId",
                principalTable: "StripeCustomer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_StripeCustomer_StripeCustomerId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_StripeCustomer_StripeCustomerId",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Customer_StripeCustomerId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "StripeCustomerId",
                table: "StripeCustomer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "StripeCustomer",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_DeletedAt",
                table: "StripeCustomer",
                column: "DeletedAt");
        }
    }
}
