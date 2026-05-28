using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoveIdentityTypeToCustomerAsCustomerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Identity_Type",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Timezone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Customer",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Type",
                table: "Customer",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customer_Type",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Identity",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Customer",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Type",
                table: "Identity",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Timezone",
                table: "Customer",
                column: "Timezone");
        }
    }
}
