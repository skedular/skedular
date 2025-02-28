using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationResourceTypeAndLocationResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationResourceType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationResourceType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationResourceType_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationResource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationResourceTypeId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationResource_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationResource_OrganizationResourceType_OrganizationResou~",
                        column: x => x.OrganizationResourceTypeId,
                        principalTable: "OrganizationResourceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLocationResource",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredLocationResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLocationResource", x => new { x.PreferredByCustomersId, x.PreferredLocationResourcesId });
                    table.ForeignKey(
                        name: "FK_CustomerLocationResource_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerLocationResource_LocationResource_PreferredLocation~",
                        column: x => x.PreferredLocationResourcesId,
                        principalTable: "LocationResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_Type",
                table: "OrganizationTag",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFeedback_Channel",
                table: "CustomerFeedback",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocationResource_PreferredLocationResourcesId",
                table: "CustomerLocationResource",
                column: "PreferredLocationResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_DeletedAt",
                table: "LocationResource",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_LocationId",
                table: "LocationResource",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_Name",
                table: "LocationResource",
                column: "Name");

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
                name: "IX_OrganizationResourceType_Type",
                table: "OrganizationResourceType",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLocationResource");

            migrationBuilder.DropTable(
                name: "LocationResource");

            migrationBuilder.DropTable(
                name: "OrganizationResourceType");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_Type",
                table: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFeedback_Channel",
                table: "CustomerFeedback");
        }
    }
}
