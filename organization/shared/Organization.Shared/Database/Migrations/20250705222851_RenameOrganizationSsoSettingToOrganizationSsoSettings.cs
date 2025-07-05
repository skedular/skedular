using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationSsoSettingToOrganizationSsoSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationSsoSetting");

            migrationBuilder.CreateTable(
                name: "OrganizationSsoSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LoginUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AppFederationMetadataUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSsoSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSsoSettings_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSettings_CreatedAt",
                table: "OrganizationSsoSettings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSettings_IsActive",
                table: "OrganizationSsoSettings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSettings_ModifiedAt",
                table: "OrganizationSsoSettings",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSettings_OrganizationId",
                table: "OrganizationSsoSettings",
                column: "OrganizationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationSsoSettings");

            migrationBuilder.CreateTable(
                name: "OrganizationSsoSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    AppFederationMetadataUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LoginUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSsoSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_CreatedAt",
                table: "OrganizationSsoSetting",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_IsActive",
                table: "OrganizationSsoSetting",
                column: "IsActive");

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
    }
}
