using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalInformtionVisibilityTypeToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberVisibilityPolicy",
                table: "Organization");

            migrationBuilder.AddColumn<string>(
                name: "PersonalInformationVisibility",
                table: "Customer",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "VISIBLE");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_PersonalInformationVisibility",
                table: "Customer",
                column: "PersonalInformationVisibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customer_PersonalInformationVisibility",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PersonalInformationVisibility",
                table: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "MemberVisibilityPolicy",
                table: "Organization",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "FULL_ACCESS");
        }
    }
}
