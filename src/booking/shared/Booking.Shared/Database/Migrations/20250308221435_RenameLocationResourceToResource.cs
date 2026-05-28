using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameLocationResourceToResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLocationResource");

            migrationBuilder.DropTable(
                name: "LocationResourceOrganizationTag");

            migrationBuilder.DropTable(
                name: "LocationResource");

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Inactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RequireBookingApproval = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerResource",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerResource", x => new { x.PreferredByCustomersId, x.PreferredResourcesId });
                    table.ForeignKey(
                        name: "FK_CustomerResource_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerResource_Resource_PreferredResourcesId",
                        column: x => x.PreferredResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagResource",
                columns: table => new
                {
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagResource", x => new { x.OrganizationTagsId, x.ResourcesId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagResource_OrganizationTag_OrganizationTagsId",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagResource_Resource_ResourcesId",
                        column: x => x.ResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerResource_PreferredResourcesId",
                table: "CustomerResource",
                column: "PreferredResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagResource_ResourcesId",
                table: "OrganizationTagResource",
                column: "ResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_DeletedAt",
                table: "Resource",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Inactive",
                table: "Resource",
                column: "Inactive");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_LocationId",
                table: "Resource",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Name",
                table: "Resource",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_RequireBookingApproval",
                table: "Resource",
                column: "RequireBookingApproval");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerResource");

            migrationBuilder.DropTable(
                name: "OrganizationTagResource");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.CreateTable(
                name: "LocationResource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Inactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RequireBookingApproval = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "LocationResourceOrganizationTag",
                columns: table => new
                {
                    LocationResourcesId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationResourceOrganizationTag", x => new { x.LocationResourcesId, x.OrganizationTagsId });
                    table.ForeignKey(
                        name: "FK_LocationResourceOrganizationTag_LocationResource_LocationRe~",
                        column: x => x.LocationResourcesId,
                        principalTable: "LocationResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationResourceOrganizationTag_OrganizationTag_Organizatio~",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocationResource_PreferredLocationResourcesId",
                table: "CustomerLocationResource",
                column: "PreferredLocationResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_DeletedAt",
                table: "LocationResource",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_Inactive",
                table: "LocationResource",
                column: "Inactive");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_LocationId",
                table: "LocationResource",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_Name",
                table: "LocationResource",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResource_RequireBookingApproval",
                table: "LocationResource",
                column: "RequireBookingApproval");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResourceOrganizationTag_OrganizationTagsId",
                table: "LocationResourceOrganizationTag",
                column: "OrganizationTagsId");
        }
    }
}
