using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotMoreField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SnapshotDefinitionLessonNumber",
                table: "LessonPlanners",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SnapshotDefinitionLessonTitle",
                table: "LessonPlanners",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnapshotDefinitionLessonNumber",
                table: "LessonPlanners");

            migrationBuilder.DropColumn(
                name: "SnapshotDefinitionLessonTitle",
                table: "LessonPlanners");
        }
    }
}
