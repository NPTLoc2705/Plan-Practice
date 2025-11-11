using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CoinAmount = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Student"),
                    CoinBalance = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    Createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttitudeTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttitudeTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttitudeTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradeLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeLevels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteractionPatterns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteractionPatterns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageFocusTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageFocusTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageFocusTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MethodTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MethodTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectiveTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectiveTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectiveTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtpVerifies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Purpose = table.Column<int>(type: "integer", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerifies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpVerifies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrderCode = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentLinkId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TransactionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PackageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreparationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreparationTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GradeLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_GradeLevels_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GradeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GradeLevelId = table.Column<int>(type: "integer", nullable: false),
                    UnitNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnitName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_GradeLevels_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GradeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    LessonNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LessonTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonDefinitions_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonPlanners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DateOfPreparation = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfTeaching = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LessonNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnitNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    LessonDefinitionId = table.Column<int>(type: "integer", nullable: true),
                    MethodTemplateId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SnapshotClassName = table.Column<string>(type: "text", nullable: true),
                    SnapshotGradeLevelName = table.Column<string>(type: "text", nullable: true),
                    SnapshotMethodName = table.Column<string>(type: "text", nullable: true),
                    SnapshotMethodDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlanners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlanners_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonPlanners_LessonDefinitions_LessonDefinitionId",
                        column: x => x.LessonDefinitionId,
                        principalTable: "LessonDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonPlanners_MethodTemplates_MethodTemplateId",
                        column: x => x.MethodTemplateId,
                        principalTable: "MethodTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonPlanners_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonPlanners_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonActivityStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    StageName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonActivityStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonActivityStages_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonAttitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    AttitudeTemplateId = table.Column<int>(type: "integer", nullable: true),
                    CustomContent = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    SnapshotName = table.Column<string>(type: "text", nullable: true),
                    SnapshotContent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonAttitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonAttitudes_AttitudeTemplates_AttitudeTemplateId",
                        column: x => x.AttitudeTemplateId,
                        principalTable: "AttitudeTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonAttitudes_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonLanguageFocusItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    LanguageFocusTypeId = table.Column<int>(type: "integer", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    SnapshotTypeName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonLanguageFocusItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonLanguageFocusItems_LanguageFocusTypes_LanguageFocusTy~",
                        column: x => x.LanguageFocusTypeId,
                        principalTable: "LanguageFocusTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonLanguageFocusItems_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    ObjectiveTemplateId = table.Column<int>(type: "integer", nullable: true),
                    CustomContent = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    SnapshotName = table.Column<string>(type: "text", nullable: true),
                    SnapshotContent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonObjectives_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonObjectives_ObjectiveTemplates_ObjectiveTemplateId",
                        column: x => x.ObjectiveTemplateId,
                        principalTable: "ObjectiveTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LessonPlannerDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlannerDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlannerDocuments_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonPreparations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    PreparationTypeId = table.Column<int>(type: "integer", nullable: true),
                    Materials = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    SnapshotTypeName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPreparations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPreparations_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                        column: x => x.PreparationTypeId,
                        principalTable: "PreparationTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LessonSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false),
                    SkillTemplateId = table.Column<int>(type: "integer", nullable: true),
                    CustomContent = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    SnapshotName = table.Column<string>(type: "text", nullable: true),
                    SnapshotDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonSkills_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonSkills_SkillTemplates_SkillTemplateId",
                        column: x => x.SkillTemplateId,
                        principalTable: "SkillTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LessonPlannerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                        column: x => x.LessonPlannerId,
                        principalTable: "LessonPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonActivityItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonActivityStageId = table.Column<int>(type: "integer", nullable: false),
                    TimeInMinutes = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    InteractionPatternId = table.Column<int>(type: "integer", nullable: true),
                    ActivityTemplateId = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    SnapshotInteractionName = table.Column<string>(type: "text", nullable: true),
                    SnapshotInteractionShortCode = table.Column<string>(type: "text", nullable: true),
                    SnapshotActivityName = table.Column<string>(type: "text", nullable: true),
                    SnapshotActivityDescription = table.Column<string>(type: "text", nullable: true),
                    SnapshotActivityContent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonActivityItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonActivityItems_ActivityTemplates_ActivityTemplateId",
                        column: x => x.ActivityTemplateId,
                        principalTable: "ActivityTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonActivityItems_InteractionPatterns_InteractionPatternId",
                        column: x => x.InteractionPatternId,
                        principalTable: "InteractionPatterns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonActivityItems_LessonActivityStages_LessonActivityStag~",
                        column: x => x.LessonActivityStageId,
                        principalTable: "LessonActivityStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    QuizId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OTPCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    QuizId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByTeacherId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UsageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MaxUsage = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizOTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizOTPs_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizOTPs_Users_CreatedByTeacherId",
                        column: x => x.CreatedByTeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    QuizId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizResults_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuizResultId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    AnswerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswers_QuizResults_QuizResultId",
                        column: x => x.QuizResultId,
                        principalTable: "QuizResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "CoinAmount", "Description", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 100, "Perfect for getting started with lesson creation", true, "Starter Pack", 9900 },
                    { 2, 500, "For frequent lesson creators", true, "Pro Creator", 39900 },
                    { 3, 1000, "Maximum value for power users", true, "Power User", 69900 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTemplates_UserId",
                table: "ActivityTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttitudeTemplates_UserId",
                table: "AttitudeTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_GradeLevelId",
                table: "Classes",
                column: "GradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeLevels_UserId",
                table: "GradeLevels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionPatterns_UserId",
                table: "InteractionPatterns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageFocusTypes_UserId",
                table: "LanguageFocusTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonActivityItems_ActivityTemplateId",
                table: "LessonActivityItems",
                column: "ActivityTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonActivityItems_InteractionPatternId",
                table: "LessonActivityItems",
                column: "InteractionPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonActivityItems_LessonActivityStageId",
                table: "LessonActivityItems",
                column: "LessonActivityStageId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonActivityStages_LessonPlannerId",
                table: "LessonActivityStages",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttitudes_AttitudeTemplateId",
                table: "LessonAttitudes",
                column: "AttitudeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttitudes_LessonPlannerId",
                table: "LessonAttitudes",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDefinitions_UnitId",
                table: "LessonDefinitions",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonLanguageFocusItems_LanguageFocusTypeId",
                table: "LessonLanguageFocusItems",
                column: "LanguageFocusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonLanguageFocusItems_LessonPlannerId",
                table: "LessonLanguageFocusItems",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonObjectives_LessonPlannerId",
                table: "LessonObjectives",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonObjectives_ObjectiveTemplateId",
                table: "LessonObjectives",
                column: "ObjectiveTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlannerDocument_LessonPlannerId",
                table: "LessonPlannerDocuments",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanners_ClassId",
                table: "LessonPlanners",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanners_LessonDefinitionId",
                table: "LessonPlanners",
                column: "LessonDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanners_MethodTemplateId",
                table: "LessonPlanners",
                column: "MethodTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanners_UnitId",
                table: "LessonPlanners",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanners_UserId",
                table: "LessonPlanners",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPreparations_LessonPlannerId",
                table: "LessonPreparations",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPreparations_PreparationTypeId",
                table: "LessonPreparations",
                column: "PreparationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonSkills_LessonPlannerId",
                table: "LessonSkills",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonSkills_SkillTemplateId",
                table: "LessonSkills",
                column: "SkillTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MethodTemplates_UserId",
                table: "MethodTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveTemplates_UserId",
                table: "ObjectiveTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifies_UserId",
                table: "OtpVerifies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerify_Email_Purpose_IsUsed",
                table: "OtpVerifies",
                columns: new[] { "Email", "Purpose", "IsUsed" });

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerify_ExpiredAt",
                table: "OtpVerifies",
                column: "ExpiredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PackageId",
                table: "Payments",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparationTypes_UserId",
                table: "PreparationTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTP_Code_IsActive_ExpiresAt",
                table: "QuizOTPs",
                columns: new[] { "OTPCode", "IsActive", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTPs_CreatedByTeacherId",
                table: "QuizOTPs",
                column: "CreatedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTPs_QuizId",
                table: "QuizOTPs",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_QuizId",
                table: "QuizResults",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_UserId",
                table: "QuizResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillTemplates_UserId",
                table: "SkillTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_GradeLevelId",
                table: "Units",
                column: "GradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_AnswerId",
                table: "UserAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_QuestionId",
                table: "UserAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_QuizResultId",
                table: "UserAnswers",
                column: "QuizResultId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonActivityItems");

            migrationBuilder.DropTable(
                name: "LessonAttitudes");

            migrationBuilder.DropTable(
                name: "LessonLanguageFocusItems");

            migrationBuilder.DropTable(
                name: "LessonObjectives");

            migrationBuilder.DropTable(
                name: "LessonPlannerDocuments");

            migrationBuilder.DropTable(
                name: "LessonPreparations");

            migrationBuilder.DropTable(
                name: "LessonSkills");

            migrationBuilder.DropTable(
                name: "OtpVerifies");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "QuizOTPs");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "ActivityTemplates");

            migrationBuilder.DropTable(
                name: "InteractionPatterns");

            migrationBuilder.DropTable(
                name: "LessonActivityStages");

            migrationBuilder.DropTable(
                name: "AttitudeTemplates");

            migrationBuilder.DropTable(
                name: "LanguageFocusTypes");

            migrationBuilder.DropTable(
                name: "ObjectiveTemplates");

            migrationBuilder.DropTable(
                name: "PreparationTypes");

            migrationBuilder.DropTable(
                name: "SkillTemplates");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "QuizResults");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "LessonPlanners");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "LessonDefinitions");

            migrationBuilder.DropTable(
                name: "MethodTemplates");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "GradeLevels");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
