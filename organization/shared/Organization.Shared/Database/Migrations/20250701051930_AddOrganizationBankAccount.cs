using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationBankAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AccountHolderName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationBankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationBankAccount_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBankAccount_CreatedAt",
                table: "OrganizationBankAccount",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBankAccount_DeletedAt",
                table: "OrganizationBankAccount",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBankAccount_IsDefault",
                table: "OrganizationBankAccount",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBankAccount_ModifiedAt",
                table: "OrganizationBankAccount",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBankAccount_Name",
                table: "OrganizationBankAccount",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationBankAccount_OrganizationId",
                table: "OrganizationBankAccount",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationBankAccount");
        }
    }
}
