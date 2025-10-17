using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIPAddressAndUserAgentFromQuizOTPAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizOTPAccess_AccessedAt",
                table: "QuizOTPAccesses");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "QuizOTPAccesses");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "QuizOTPAccesses");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AccessedAt",
                table: "QuizOTPAccesses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "timezone('utc', now())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AccessedAt",
                table: "QuizOTPAccesses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "timezone('utc', now())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "QuizOTPAccesses",
                type: "character varying(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "QuizOTPAccesses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOTPAccess_AccessedAt",
                table: "QuizOTPAccesses",
                column: "AccessedAt");
        }
    }
}
