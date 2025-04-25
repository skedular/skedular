using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class PreparingStripeCustomerMigrationPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripeCustomerId",
                table: "Organization",
                newName: "StripeCustomerIdTemp");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization",
                newName: "IX_Organization_StripeCustomerIdTemp");

            migrationBuilder.CreateTable(
                name: "StripeCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeCustomer", x => x.Id);
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
                name: "IX_StripeCustomer_StripeCustomerId",
                table: "StripeCustomer",
                column: "StripeCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeCustomer");

            migrationBuilder.RenameColumn(
                name: "StripeCustomerIdTemp",
                table: "Organization",
                newName: "StripeCustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_StripeCustomerIdTemp",
                table: "Organization",
                newName: "IX_Organization_StripeCustomerId");
        }
    }
}
