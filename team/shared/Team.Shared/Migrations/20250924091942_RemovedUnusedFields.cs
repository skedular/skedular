using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customer_PhoneNumber",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
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
                name: "PhoneNumber",
                table: "Customer",
                type: "character varying(64)",
                maxLength: 64,
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
                name: "IX_Customer_PhoneNumber",
                table: "Customer",
                column: "PhoneNumber");
        }
    }
}
