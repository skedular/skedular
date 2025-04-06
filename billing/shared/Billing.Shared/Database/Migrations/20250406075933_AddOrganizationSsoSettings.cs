using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationSsoSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationSsoSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LoginUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AppFederationMetadataUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSsoSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_CreatedAt",
                table: "OrganizationSsoSetting",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_ModifiedAt",
                table: "OrganizationSsoSetting",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationSsoSetting");
        }
    }
}
