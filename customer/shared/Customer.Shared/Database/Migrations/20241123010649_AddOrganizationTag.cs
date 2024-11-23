using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationTag_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrganizationTag",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredOrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrganizationTag", x => new { x.PreferredByCustomersId, x.PreferredOrganizationTagsId });
                    table.ForeignKey(
                        name: "FK_CustomerOrganizationTag_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrganizationTag_OrganizationTag_PreferredOrganizati~",
                        column: x => x.PreferredOrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrganizationTag_PreferredOrganizationTagsId",
                table: "CustomerOrganizationTag",
                column: "PreferredOrganizationTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_DeletedAt",
                table: "OrganizationTag",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_Name",
                table: "OrganizationTag",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_OrganizationId",
                table: "OrganizationTag",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrganizationTag");

            migrationBuilder.DropTable(
                name: "OrganizationTag");
        }
    }
}
