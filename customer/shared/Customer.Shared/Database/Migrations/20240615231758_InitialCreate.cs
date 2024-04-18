using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Topic = table.Column<string>(type: "character varying(249)", maxLength: 249, nullable: false),
                    Headers = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    Key = table.Column<byte[]>(type: "bytea", nullable: false),
                    Payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastRetry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ProcessingErrors = table.Column<string>(type: "character varying(102400)", maxLength: 102400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    GivenName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FamilyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl24 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl32 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl48 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl72 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl192 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl512 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Timezone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Locale = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsOrganizationOnboardingDone = table.Column<bool>(type: "boolean", nullable: true),
                    IsLocationOnboardingDone = table.Column<bool>(type: "boolean", nullable: true),
                    IsDefaultOrganizationOnboardingDone = table.Column<bool>(type: "boolean", nullable: true),
                    IsDefaultLocationOnboardingDone = table.Column<bool>(type: "boolean", nullable: true),
                    IsPreferredZoneOnboardingDone = table.Column<bool>(type: "boolean", nullable: true),
                    IsPreferredDeskOnboardingDone = table.Column<bool>(type: "boolean", nullable: true),
                    DefaultOrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Organization_DefaultOrganizationId",
                        column: x => x.DefaultOrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerFeedback",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "character varying(10240)", maxLength: 10240, nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerFeedback_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationMember_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLocation",
                columns: table => new
                {
                    DefaultLocationsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    DefaultedByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLocation", x => new { x.DefaultLocationsId, x.DefaultedByCustomersId });
                    table.ForeignKey(
                        name: "FK_CustomerLocation_Customer_DefaultedByCustomersId",
                        column: x => x.DefaultedByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerLocation_Location_DefaultLocationsId",
                        column: x => x.DefaultLocationsId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Desk",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desk_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationMember_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationTag_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTeam",
                columns: table => new
                {
                    DefaultTeamsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    DefaultedByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTeam", x => new { x.DefaultTeamsId, x.DefaultedByCustomersId });
                    table.ForeignKey(
                        name: "FK_CustomerTeam_Customer_DefaultedByCustomersId",
                        column: x => x.DefaultedByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTeam_Team_DefaultTeamsId",
                        column: x => x.DefaultTeamsId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: true),
                    TeamId = table.Column<string>(type: "character varying(100)", nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationMemberId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_OrganizationMember_OrganizationMemberId",
                        column: x => x.OrganizationMemberId,
                        principalTable: "OrganizationMember",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamMember_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerDesk",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredDesksId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDesk", x => new { x.PreferredByCustomersId, x.PreferredDesksId });
                    table.ForeignKey(
                        name: "FK_CustomerDesk_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerDesk_Desk_PreferredDesksId",
                        column: x => x.PreferredDesksId,
                        principalTable: "Desk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLocationTag",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredLocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLocationTag", x => new { x.PreferredByCustomersId, x.PreferredLocationTagsId });
                    table.ForeignKey(
                        name: "FK_CustomerLocationTag_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerLocationTag_LocationTag_PreferredLocationTagsId",
                        column: x => x.PreferredLocationTagsId,
                        principalTable: "LocationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeskLocationTag",
                columns: table => new
                {
                    TaggedDesksId = table.Column<string>(type: "character varying(100)", nullable: false),
                    TagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskLocationTag", x => new { x.TaggedDesksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_DeskLocationTag_Desk_TaggedDesksId",
                        column: x => x.TaggedDesksId,
                        principalTable: "Desk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeskLocationTag_LocationTag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "LocationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_DefaultOrganizationId",
                table: "Customer",
                column: "DefaultOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_DeletedAt",
                table: "Customer",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Designation",
                table: "Customer",
                column: "Designation");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_FamilyName",
                table: "Customer",
                column: "FamilyName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_GivenName",
                table: "Customer",
                column: "GivenName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Locale",
                table: "Customer",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MiddleName",
                table: "Customer",
                column: "MiddleName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customer",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Timezone",
                table: "Customer",
                column: "Timezone");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Title",
                table: "Customer",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDesk_PreferredDesksId",
                table: "CustomerDesk",
                column: "PreferredDesksId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFeedback_CustomerId",
                table: "CustomerFeedback",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocation_DefaultedByCustomersId",
                table: "CustomerLocation",
                column: "DefaultedByCustomersId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocationTag_PreferredLocationTagsId",
                table: "CustomerLocationTag",
                column: "PreferredLocationTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTeam_DefaultedByCustomersId",
                table: "CustomerTeam",
                column: "DefaultedByCustomersId");

            migrationBuilder.CreateIndex(
                name: "IX_Desk_DeletedAt",
                table: "Desk",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Desk_LocationId",
                table: "Desk",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Desk_Name",
                table: "Desk",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DeskLocationTag_TagsId",
                table: "DeskLocationTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_CustomerId",
                table: "Identity",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Email",
                table: "Identity",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_EmailVerified",
                table: "Identity",
                column: "EmailVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DeletedAt",
                table: "Location",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Name",
                table: "Location",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Location_OrganizationId",
                table: "Location",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId",
                table: "LocationMember",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_DeletedAt",
                table: "LocationMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_LocationId",
                table: "LocationMember",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_MembershipType",
                table: "LocationMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTag_DeletedAt",
                table: "LocationTag",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTag_LocationId",
                table: "LocationTag",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTag_Name",
                table: "LocationTag",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_DeletedAt",
                table: "Organization",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_CustomerId",
                table: "OrganizationMember",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_DeletedAt",
                table: "OrganizationMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_OrganizationId",
                table: "OrganizationMember",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_LastRetry",
                table: "Outbox",
                column: "LastRetry");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_RetryCount",
                table: "Outbox",
                column: "RetryCount");

            migrationBuilder.CreateIndex(
                name: "IX_Team_DeletedAt",
                table: "Team",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Team_Name",
                table: "Team",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Team_OrganizationId",
                table: "Team",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CustomerId",
                table: "TeamMember",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_DeletedAt",
                table: "TeamMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_MembershipType",
                table: "TeamMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_OrganizationMemberId",
                table: "TeamMember",
                column: "OrganizationMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMember",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerDesk");

            migrationBuilder.DropTable(
                name: "CustomerFeedback");

            migrationBuilder.DropTable(
                name: "CustomerLocation");

            migrationBuilder.DropTable(
                name: "CustomerLocationTag");

            migrationBuilder.DropTable(
                name: "CustomerTeam");

            migrationBuilder.DropTable(
                name: "DeskLocationTag");

            migrationBuilder.DropTable(
                name: "Identity");

            migrationBuilder.DropTable(
                name: "LocationMember");

            migrationBuilder.DropTable(
                name: "Outbox");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "Desk");

            migrationBuilder.DropTable(
                name: "LocationTag");

            migrationBuilder.DropTable(
                name: "OrganizationMember");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Organization");
        }
    }
}
