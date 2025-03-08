using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrganizationResourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationResource_OrganizationResourceType_OrganizationResou~",
                table: "LocationResource");

            migrationBuilder.DropTable(
                name: "OrganizationResourceType");

            migrationBuilder.DropIndex(
                name: "IX_LocationResource_OrganizationResourceTypeId",
                table: "LocationResource");

            migrationBuilder.DropColumn(
                name: "OrganizationResourceTypeId",
                table: "LocationResource");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationResourceTypeId",
                table: "LocationResource",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrganizationResourceType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SystemType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationResourceType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationResourceType_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_OrganizationResourceTypeId",
                table: "LocationResource",
                column: "OrganizationResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_DeletedAt",
                table: "OrganizationResourceType",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_Name",
                table: "OrganizationResourceType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_OrganizationId",
                table: "OrganizationResourceType",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_SystemType",
                table: "OrganizationResourceType",
                column: "SystemType");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationResource_OrganizationResourceType_OrganizationResou~",
                table: "LocationResource",
                column: "OrganizationResourceTypeId",
                principalTable: "OrganizationResourceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
