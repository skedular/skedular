using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customer_FamilyName",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_GivenName",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_MiddleName",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Name",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl192",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl24",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl32",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl48",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl512",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl72",
                table: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Customer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Customer",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl192",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl24",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl32",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl48",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl512",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl72",
                table: "Customer",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_FamilyName",
                table: "Customer",
                column: "FamilyName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_GivenName",
                table: "Customer",
                column: "GivenName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MiddleName",
                table: "Customer",
                column: "MiddleName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customer",
                column: "Name");
        }
    }
}
