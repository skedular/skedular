using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Organization.Shared.Database.Migrations
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
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndustryMainCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryMainCategory", x => x.Id);
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
                name: "TermsOfUse",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Terms = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsOfUse", x => x.Id);
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
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
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
                name: "IndustrySubCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IndustryMainCategoryId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrySubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustrySubCategory_IndustryMainCategory_IndustryMainCatego~",
                        column: x => x.IndustryMainCategoryId,
                        principalTable: "IndustryMainCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AgreedToTermsOfUse = table.Column<bool>(type: "boolean", nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    HasAttachedPaymentMethod = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentMethodEventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DailyMemberCountLastRecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TermsOfUseId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_TermsOfUse_TermsOfUseId",
                        column: x => x.TermsOfUseId,
                        principalTable: "TermsOfUse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    From = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyMemberCountRecording",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMemberCountRecording", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMemberCountRecording_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndustrySubCategoryOrganization",
                columns: table => new
                {
                    IndustrySubCategoriesId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrySubCategoryOrganization", x => new { x.IndustrySubCategoriesId, x.OrganizationsId });
                    table.ForeignKey(
                        name: "FK_IndustrySubCategoryOrganization_IndustrySubCategory_Industr~",
                        column: x => x.IndustrySubCategoriesId,
                        principalTable: "IndustrySubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndustrySubCategoryOrganization_Organization_OrganizationsId",
                        column: x => x.OrganizationsId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinInvitation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(100)", nullable: false),
                    InviteeId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinInvitation_Customer_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinInvitation_Customer_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JoinInvitation_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                name: "OrganizationOffering",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    UnitPrice = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationOffering", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationOffering_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationOfferingActiveMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationMemberId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationOfferingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationOfferingActiveMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationOfferingActiveMember_OrganizationMember_Organiz~",
                        column: x => x.OrganizationMemberId,
                        principalTable: "OrganizationMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationOfferingActiveMember_OrganizationOffering_Organ~",
                        column: x => x.OrganizationOfferingId,
                        principalTable: "OrganizationOffering",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IndustryMainCategory",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { "-PfBGjlmBqLSUhkj5HGfP", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Entertainment" },
                    { "08-giYmx7ja5wepmU10j5", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Public Administration" },
                    { "08ILcal4_is07nQlMRtae", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Software & IT Services" },
                    { "0NXImArR8CXFDC9SmmwFn", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Arts" },
                    { "5y-GA2lrc3pk5fHG-3YIy", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Health Care" },
                    { "620m_qu0dee49rW0104aI", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Nonprofit" },
                    { "6pu5HDPw5APjFvcT-eL0Q", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Real Estate" },
                    { "882bUq1BWqJecAZxMOr51", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Agriculture" },
                    { "9zzrzqbocNXiv9_OLRgtE", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Transportation & Logistics" },
                    { "eO9IbE_ssHvels5sLHtob", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Wellness & Fitness" },
                    { "gWWnxzMaGrBIp5JsKqTUV", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Media & Communications" },
                    { "hAxKDrJiJmHK__0M_ewMu", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Public Safety" },
                    { "kamIaBPmTt1gZCRjqTlvG", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Energy & Mining" },
                    { "LFaZVLT6kUWs-N_tIKdvv", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Manufacturing" },
                    { "lg-BOyEpbyAi_AGt3EeNX", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Consumer Goods" },
                    { "pXKhQk06h0DDf6cYf93C1", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Construction" },
                    { "S1mxU6bv5ktRVVIN3AA4K", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Design" },
                    { "s3_JhMKyBezxJzRJq9BO0", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Hardware & Networking" },
                    { "vS4OynyP3n3kjc3l-bmGS", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Retail" },
                    { "wzCmjl5D_n22GAmJquRWB", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Education" },
                    { "xkjX-i2E2Bc6tH2KjCaTu", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Corporate Services" },
                    { "xY4RDCWRG5G2fOEdNXPng", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Recreation & Travel" },
                    { "zLuSwB4G_EuG4YueixLF0", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Legal" },
                    { "zwanHBU5wvwbQrGspAXTb", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "Finance" }
                });

            migrationBuilder.InsertData(
                table: "TermsOfUse",
                columns: new[] { "Id", "Active", "CreatedAt", "DeletedAt", "ModifiedAt", "Terms" },
                values: new object[] { "VHzIH3DC09QJrOrCV-PnU", true, new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "I verify that I am an authorized representative of this organization and have the right to act on its behalf in the creation and management of this page. The organization and I agree to the additional terms for Pages." });

            migrationBuilder.InsertData(
                table: "IndustrySubCategory",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IndustryMainCategoryId", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { "_GJ2pKqtFHpFFWeq5uP7A", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Management Consulting" },
                    { "_TTaYtfsEUmytV_9n2PQy", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Consumer Services" },
                    { "_tUue6Gl-953pMKTqN7TI", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Mobile Games" },
                    { "0LvvVx4gPaRD73p9dAKA6", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Luxury Goods & Jewelry" },
                    { "1OQUodxh6dXzBAzyhE_In", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "882bUq1BWqJecAZxMOr51", null, "Fishery" },
                    { "1vbfjkAaHk0w6svYoDBjd", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Investment Banking" },
                    { "2Dn5XQlDSvNIuPcNz1Z2_", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "Government Administration" },
                    { "2KfFED1vb65J4w0HIAH4o", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "s3_JhMKyBezxJzRJq9BO0", null, "Computer Networking" },
                    { "2wvDDgBTMDs5pDVlbS1OX", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Financial Services" },
                    { "3ALho9Wdd4nPstkcPCaqo", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "s3_JhMKyBezxJzRJq9BO0", null, "Nanotechnologie" },
                    { "3QLolUIA1CaLD1KV01QmU", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Museums & Institutions" },
                    { "4b5K73V1j6E0pXpTT6EF2", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "International Trade & Development" },
                    { "4fHASimX1R3Kb-soGZcTn", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "s3_JhMKyBezxJzRJq9BO0", null, "Computer Hardware" },
                    { "4mEHLACzvNFW_GUDtRxjy", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "9zzrzqbocNXiv9_OLRgtE", null, "Warehousing" },
                    { "4NQjYM6jdfnKVRwz5vJzK", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Cosmetics" },
                    { "5_xJDrAIJrMsS8SfvyJ9z", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Machinery" },
                    { "68AhtustNiXT8H107NbXo", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Events Services" },
                    { "6AUKesJxOibCNaXiVxejR", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "Political Organization" },
                    { "6FC0NBRxizMTAvhm-huwF", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Airlines/Aviation" },
                    { "6fV2OxmlTsfoOVVJ9KFfk", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08ILcal4_is07nQlMRtae", null, "Computer & Network Security" },
                    { "7HyAeDaXYrv24Mfsr9Vs8", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Philanthropy" },
                    { "8Sbp7Tb4ktevjRWWruzye", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "9zzrzqbocNXiv9_OLRgtE", null, "Maritime" },
                    { "8sdTPVkp6YM4gwIDd6yTL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Plastics" },
                    { "8y4ShEofzG0tIDWcDFRsL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Individual & Family Services" },
                    { "9hvvmVHeIYptCbkUhj0jH", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "9zzrzqbocNXiv9_OLRgtE", null, "Package/Freight Delivery" },
                    { "9lwcTkiRLUx1-yK8CsRc3", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zLuSwB4G_EuG4YueixLF0", null, "Law Practice" },
                    { "9SqM8gfgtFgKDpUr1cNco", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "vS4OynyP3n3kjc3l-bmGS", null, "Retail" },
                    { "Anp0oogRUzx-UZ1clwGAa", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Accounting" },
                    { "bHXQAM_dBX_jf3YaPVtzn", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "eO9IbE_ssHvels5sLHtob", null, "Health, Wellness & Fitness" },
                    { "BsRUgUyxfncNkCq4wpLJj", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Motion Pictures & Film" },
                    { "btsPkeHzW4sNny11BSvof", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "S1mxU6bv5ktRVVIN3AA4K", null, "Graphic Design" },
                    { "CAtDPbb6rU1Vz2gPAhPlv", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "hAxKDrJiJmHK__0M_ewMu", null, "Law Enforcement" },
                    { "cE5QYVgV7Rug2iEmhPSLp", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Hospitality" },
                    { "Cv4P4IfMXEFDds1WILHMf", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Consumer Goods" },
                    { "cyxCXes3Pi4QN_DoTq7QQ", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Tobacco" },
                    { "d6K3MC3hA_zZXsGRMyHSM", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Railroad Manufacture" },
                    { "dkD2WRvcb33CAjsB35l1r", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Publishing" },
                    { "DoZAnGTFeGb2lrW--1pJi", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Furniture" },
                    { "DRD09Q2dKJGDJSp_0_64W", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08ILcal4_is07nQlMRtae", null, "Computer Software" },
                    { "dwWQZ-qlYhqDloUB0yQhW", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Music" },
                    { "dxmzLuPtXJqUGc4rRYl_8", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Entertainment" },
                    { "DXvW70szUeBs2Qt_FUTyn", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Apparel & Fashion" },
                    { "EBrEnqmLfty7fdbbUCe4f", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "0NXImArR8CXFDC9SmmwFn", null, "Performing Arts" },
                    { "EIJHhNw_wpIVL3pA1OOZr", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Insurance" },
                    { "esqKnQGv3x5aqWniYO6qn", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Fundraising" },
                    { "ETHXo2XAWdvAVeNraKr7o", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Pharmaceuticals" },
                    { "eTmgDsQHooF8yfjQibuZx", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Restaurants" },
                    { "Fl8iL9jTkTcDLniwG8PjM", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Industrial Automation" },
                    { "fMq27yPPSWdyt56OHGARE", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Staffing & Recruiting" },
                    { "GbvxgUqTZIi_eJLzHk101", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Investment Management" },
                    { "Gi_MSree2JJPgxibLvwS-", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Civic & Social Organization" },
                    { "gojKcjl4sTRVokd6Fwm5Q", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Glass, Ceramics & Concrete" },
                    { "H9-dThKHpcKEPj_ncmau8", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Sports" },
                    { "HauXa9RY9QSr8a54ByFwj", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "pXKhQk06h0DDf6cYf93C1", null, "Building Materials" },
                    { "HDkZbXrHsEz9108zjzU3N", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "882bUq1BWqJecAZxMOr51", null, "Ranching" },
                    { "HEN-ijto0K1Sf_OciHDIq", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "S1mxU6bv5ktRVVIN3AA4K", null, "Design" },
                    { "hJNIh-Im_f7clkYZmZBRE", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "pXKhQk06h0DDf6cYf93C1", null, "Civil Engineering" },
                    { "HkVmjXmMbNK5rW_yErWrB", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Consumer Electronics" },
                    { "hMrZmeoHvKaOy0ufUZa35", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Banking" },
                    { "hzE99r9OYpChAm-J-I95R", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Security & Investigations" },
                    { "ideEWzCBrRa2lyrKp2lAb", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "kamIaBPmTt1gZCRjqTlvG", null, "Utilities" },
                    { "ih37Ze5aaOGfG7TYXUsbv", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "S1mxU6bv5ktRVVIN3AA4K", null, "Architecture & Planning" },
                    { "jCLq3vnUK-nibBg1JOK_O", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "hAxKDrJiJmHK__0M_ewMu", null, "Military" },
                    { "jDUPoojddO2IZNNk7HLn_", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "9zzrzqbocNXiv9_OLRgtE", null, "Import & Export" },
                    { "jNfz-hPjT4aCuU6Ul0uee", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "vS4OynyP3n3kjc3l-bmGS", null, "Wholesale" },
                    { "K_rfAbaRdODY0FzSi4eCP", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Gambling & Casinos" },
                    { "K3SqEjUcLSaj-OASf-Vfn", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Executive Office" },
                    { "kc6a_bX7NdI8XLPo9O_OQ", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Veterinary" },
                    { "KfojPdlEXy5bhjhv9Buti", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Electrical & Electronic Manufacturing" },
                    { "kmCWCF7gBidij5s3qIBFa", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "kamIaBPmTt1gZCRjqTlvG", null, "Mining & Metals" },
                    { "KplQglXodzYl59gK_epnf", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Think Tanks" },
                    { "Kzq6AwGAsKU2tEky6V6Ym", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "vS4OynyP3n3kjc3l-bmGS", null, "Supermarkets" },
                    { "LG7xy63xPAdigL2noaJ3Q", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Mental Health Care" },
                    { "LIY449onOi0tKM7y5Lf2E", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zLuSwB4G_EuG4YueixLF0", null, "Legal Services" },
                    { "LnRhNtxZehp9pPTaHrVcx", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Leisure, Travel & Tourism" },
                    { "lrjZuf6_7u2jIgNHNIYEx", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Media Production" },
                    { "LUyonPGmj7DGOXCnvRKn_", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08ILcal4_is07nQlMRtae", null, "Information Technology & Services" },
                    { "MbTSEkTyZa4sCNPlhzybY", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "0NXImArR8CXFDC9SmmwFn", null, "Fine Art" },
                    { "mHMMNmKLOEtOvMHM3Vv53", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "s3_JhMKyBezxJzRJq9BO0", null, "Telecommunications" },
                    { "MM_23S9wXuT1XN0hvCpqD", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "Judiciary" },
                    { "mrVhAaI2m_3ZA7xnddJFt", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Textiles" },
                    { "MXUwHkgay_UZXX2AzY1FU", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Business Supplies & Equipment" },
                    { "mZE0AvfFQrQAPbExKJTZK", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Shipbuilding" },
                    { "N9NMqIS5yQTzU5GYDLGaS", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "eO9IbE_ssHvels5sLHtob", null, "Alternative Medicine" },
                    { "ngm0upQBvO_Vn-4gsEEJx", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "hAxKDrJiJmHK__0M_ewMu", null, "Public Safety" },
                    { "nMV4PpF25mWpYwN-tSIB0", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Chemicals" },
                    { "nnUTfoWcex-OQST0GSBKx", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "s3_JhMKyBezxJzRJq9BO0", null, "Wireless" },
                    { "NwRiGUxTpWjBD_QLTmgb1", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zLuSwB4G_EuG4YueixLF0", null, "Alternative Dispute Resolution" },
                    { "oCfNjmm0Q5YCrR_qnXhfv", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "9zzrzqbocNXiv9_OLRgtE", null, "Transportation/Trucking/Railroad" },
                    { "oGHZiJyHGGZRlNz9ooLj_", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Information Services" },
                    { "OhG4N-hdXRWFsnO3mQQ_K", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "s3_JhMKyBezxJzRJq9BO0", null, "Semiconductors" },
                    { "P_Qo9uJlw5ERFBfAxI5PV", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Automotive" },
                    { "P5qA_npwKevf0IuQQ-o8V", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Facilities Services" },
                    { "PDzsPTIM6XZ6wG04mfB1o", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "6pu5HDPw5APjFvcT-eL0Q", null, "Commercial Real Estate" },
                    { "PFk8EMo95ki7CBxEFOhgp", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "Legislative Office" },
                    { "PGAC2vXJHtktpqxfiw5TQ", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Defense & Space" },
                    { "pGmd11WrkICGA8ZbHSJXZ", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Animation" },
                    { "pHYIZBESp80D3ZV4Sn7rV", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "882bUq1BWqJecAZxMOr51", null, "Dairy" },
                    { "PLW5y9Q2M5jZxxzYsZ1zL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Computer Games" },
                    { "PmYm5CdXqxfmNJU9KYpQK", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Wine and Spirits" },
                    { "pPypbwxYDCO_vayD_m1vR", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "International Affairs" },
                    { "psDvbeMLWDuDiUG6RLtQm", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Online Media" },
                    { "PwffwJ0sXibSyQ4oXdBk4", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Human Resources" },
                    { "Qc81ixxiDmrK2nOdrAmmL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Medical Device" },
                    { "qh8mwzmfi1NuEjGc8z5S0", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Paper & Forest Products" },
                    { "QZvLjlV-l-WUMAsoAVBj-", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Marketing & Advertising" },
                    { "R6UJ7bC5s5RET284J7fTI", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "Government Relations" },
                    { "Rc0rGZQCEu39Bw6WdutRL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Writing & Editing" },
                    { "rC3xY9zAu8f6RCUPSKVjD", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Food Production" },
                    { "RgKCDwUMXB5cLaNI5YZ9_", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Public Relations & Communications" },
                    { "RhArj4cCu9IkPqCmTXpnL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08-giYmx7ja5wepmU10j5", null, "Public Policy" },
                    { "S4hTeNXKIRL84bRRzli1k", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Aviation & Aerospace" },
                    { "sk3VI48ZM0PXC5yKnB91Z", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Translation & Localization" },
                    { "SmwTdBTItdXOsYU1tIBhL", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Libraries" },
                    { "sr2AuEYun_pZUWZI7gQcz", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Printing" },
                    { "tkF5aFhlRnPPhRsodehd9", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Environmental Services" },
                    { "TKY-GyWAwwhNKtSHVgfjS", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "0NXImArR8CXFDC9SmmwFn", null, "Photography" },
                    { "tkzVx19aJOia3kGSyaug0", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Packaging & Containers" },
                    { "tOLdxfJbbyZseUsruul4F", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "6pu5HDPw5APjFvcT-eL0Q", null, "Real Estate" },
                    { "uIE9NifoiClxefpceV_X2", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Religious Institutions" },
                    { "UugkhQXKrqMj0btvXi7S8", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "9zzrzqbocNXiv9_OLRgtE", null, "Logistics & Supply Chain" },
                    { "UxWZXcAvWUEMrwqvlLKcR", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xY4RDCWRG5G2fOEdNXPng", null, "Recreational Facilities & Services" },
                    { "v36gThLN0qbHByjyu_Gl_", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Market Research" },
                    { "vgBXbrQLaCmsOQVfy68ob", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Non-Profit Organization Management" },
                    { "vggS9I-URCNvKv5lDV4Qv", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Hospital & Health Care" },
                    { "Wcxg0azu7qwIOnESWfKD6", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Outsourcing/Offshoring" },
                    { "WF-XxhBCdD2UN1tZXzWXd", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "620m_qu0dee49rW0104aI", null, "Program Development" },
                    { "WmZvpAjtGwZ_UBAzXE8qR", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Venture Capital & Private Equity" },
                    { "WTd59z5le2R3ljmwoSOMo", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "0NXImArR8CXFDC9SmmwFn", null, "Arts & Crafts" },
                    { "wuaS-83ScePOPqZDXbSWd", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "kamIaBPmTt1gZCRjqTlvG", null, "Oil & Energy" },
                    { "WYpGJyTVuuM92igAmspbE", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Food & Beverages" },
                    { "XaZJbtarjgHa9aJfGOllq", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Renewables & Environment" },
                    { "xGRNZeN51DBhyNcvIDpmp", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Biotechnology" },
                    { "XoecAXYxdwsdOQyH44as-", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "xkjX-i2E2Bc6tH2KjCaTu", null, "Professional Training & Coaching" },
                    { "XOKkzryiG28mrlGfF5_H4", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "LFaZVLT6kUWs-N_tIKdvv", null, "Mechanical or Industrial Engineering" },
                    { "Xoox6447RFMKGcnSQRyCM", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "lg-BOyEpbyAi_AGt3EeNX", null, "Sporting Goods" },
                    { "xTZpJWTpdotpEglrHqPvd", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "-PfBGjlmBqLSUhkj5HGfP", null, "Broadcast Media" },
                    { "xVSkoLNA2s6T9Wo1uFD6R", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "882bUq1BWqJecAZxMOr51", null, "Farming" },
                    { "Y-EXiW8yWEtnM6MdMq3MD", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "08ILcal4_is07nQlMRtae", null, "Internet" },
                    { "yXJjHKfGxQkrrX8IIQBoX", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "5y-GA2lrc3pk5fHG-3YIy", null, "Medical Practice" },
                    { "Z7swakPNl1_vEUHT-uip3", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "zwanHBU5wvwbQrGspAXTb", null, "Capital Markets" },
                    { "zfC86Kgwc8f9fzP5latH9", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "gWWnxzMaGrBIp5JsKqTUV", null, "Newspapers" },
                    { "zQRObEMkSHN8QOtTZBwkJ", new DateTimeOffset(new DateTime(2024, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "pXKhQk06h0DDf6cYf93C1", null, "Construction" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_DeletedAt",
                table: "Booking",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_From",
                table: "Booking",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_OrganizationId",
                table: "Booking",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_To",
                table: "Booking",
                column: "To");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_DeletedAt",
                table: "Customer",
                column: "DeletedAt");

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

            migrationBuilder.CreateIndex(
                name: "IX_DailyMemberCountRecording_Count",
                table: "DailyMemberCountRecording",
                column: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMemberCountRecording_Date",
                table: "DailyMemberCountRecording",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMemberCountRecording_DeletedAt",
                table: "DailyMemberCountRecording",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMemberCountRecording_OrganizationId",
                table: "DailyMemberCountRecording",
                column: "OrganizationId");

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
                name: "IX_IndustryMainCategory_DeletedAt",
                table: "IndustryMainCategory",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrySubCategory_DeletedAt",
                table: "IndustrySubCategory",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrySubCategory_IndustryMainCategoryId",
                table: "IndustrySubCategory",
                column: "IndustryMainCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrySubCategoryOrganization_OrganizationsId",
                table: "IndustrySubCategoryOrganization",
                column: "OrganizationsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_CreatedById",
                table: "JoinInvitation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_DeletedAt",
                table: "JoinInvitation",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Email",
                table: "JoinInvitation",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_InviteeId",
                table: "JoinInvitation",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_OrganizationId",
                table: "JoinInvitation",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Status",
                table: "JoinInvitation",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DeletedAt",
                table: "Location",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_OrganizationId",
                table: "Location",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_About",
                table: "Organization",
                column: "About");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_DailyMemberCountLastRecordedAt",
                table: "Organization",
                column: "DailyMemberCountLastRecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_DeletedAt",
                table: "Organization",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TermsOfUseId",
                table: "Organization",
                column: "TermsOfUseId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Website",
                table: "Organization",
                column: "Website");

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
                name: "IX_OrganizationOffering_AutoRenew",
                table: "OrganizationOffering",
                column: "AutoRenew");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_Code",
                table: "OrganizationOffering",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_DeletedAt",
                table: "OrganizationOffering",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_End",
                table: "OrganizationOffering",
                column: "End");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_OrganizationId",
                table: "OrganizationOffering",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_Start",
                table: "OrganizationOffering",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_Start_End",
                table: "OrganizationOffering",
                columns: new[] { "Start", "End" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_UnitPrice",
                table: "OrganizationOffering",
                column: "UnitPrice");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingActiveMember_OrganizationMemberId",
                table: "OrganizationOfferingActiveMember",
                column: "OrganizationMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingActiveMember_OrganizationOfferingId",
                table: "OrganizationOfferingActiveMember",
                column: "OrganizationOfferingId");

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
                name: "IX_Team_OrganizationId",
                table: "Team",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TermsOfUse_DeletedAt",
                table: "TermsOfUse",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "DailyMemberCountRecording");

            migrationBuilder.DropTable(
                name: "Identity");

            migrationBuilder.DropTable(
                name: "IndustrySubCategoryOrganization");

            migrationBuilder.DropTable(
                name: "JoinInvitation");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "OrganizationOfferingActiveMember");

            migrationBuilder.DropTable(
                name: "Outbox");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "IndustrySubCategory");

            migrationBuilder.DropTable(
                name: "OrganizationMember");

            migrationBuilder.DropTable(
                name: "OrganizationOffering");

            migrationBuilder.DropTable(
                name: "IndustryMainCategory");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "TermsOfUse");
        }
    }
}
