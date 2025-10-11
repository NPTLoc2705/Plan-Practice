using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizOTPFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "QuizOTPAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OTPId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    IPAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizOTPAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizOTPAccesses_QuizOTPs_OTPId",
                        column: x => x.OTPId,
                        principalTable: "QuizOTPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizOTPAccesses_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTPAccess_AccessedAt",
                table: "QuizOTPAccesses",
                column: "AccessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTPAccess_OTP_Student",
                table: "QuizOTPAccesses",
                columns: new[] { "OTPId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTPAccesses_StudentId",
                table: "QuizOTPAccesses",
                column: "StudentId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizOTPAccesses");

            migrationBuilder.DropTable(
                name: "QuizOTPs");
        }
    }
}
