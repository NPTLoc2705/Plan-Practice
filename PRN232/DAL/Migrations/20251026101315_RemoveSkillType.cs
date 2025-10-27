using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSkillType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillTemplates_SkillTypes_SkillTypeId",
                table: "SkillTemplates");

            migrationBuilder.DropTable(
                name: "SkillTypes");

            migrationBuilder.DropIndex(
                name: "IX_SkillTemplates_SkillTypeId",
                table: "SkillTemplates");

            migrationBuilder.DropColumn(
                name: "SkillTypeId",
                table: "SkillTemplates");

            migrationBuilder.DropColumn(
                name: "SnapshotSkillType",
                table: "LessonSkills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SkillTypeId",
                table: "SkillTemplates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SnapshotSkillType",
                table: "LessonSkills",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SkillTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillTemplates_SkillTypeId",
                table: "SkillTemplates",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillTypes_UserId",
                table: "SkillTypes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillTemplates_SkillTypes_SkillTypeId",
                table: "SkillTemplates",
                column: "SkillTypeId",
                principalTable: "SkillTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
