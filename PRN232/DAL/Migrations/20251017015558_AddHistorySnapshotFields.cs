using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorySnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotDescription",
                table: "LessonSkills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotName",
                table: "LessonSkills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotSkillType",
                table: "LessonSkills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PreparationTypeId",
                table: "LessonPreparations",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotTypeName",
                table: "LessonPreparations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotClassName",
                table: "LessonPlanners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotGradeLevelName",
                table: "LessonPlanners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotMethodDescription",
                table: "LessonPlanners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotMethodName",
                table: "LessonPlanners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotContent",
                table: "LessonObjectives",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotName",
                table: "LessonObjectives",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotTypeName",
                table: "LessonLanguageFocusItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotContent",
                table: "LessonAttitudes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotName",
                table: "LessonAttitudes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotInteractionName",
                table: "LessonActivityItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotInteractionShortCode",
                table: "LessonActivityItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations",
                column: "PreparationTypeId",
                principalTable: "PreparationTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations");

            migrationBuilder.DropColumn(
                name: "SnapshotDescription",
                table: "LessonSkills");

            migrationBuilder.DropColumn(
                name: "SnapshotName",
                table: "LessonSkills");

            migrationBuilder.DropColumn(
                name: "SnapshotSkillType",
                table: "LessonSkills");

            migrationBuilder.DropColumn(
                name: "SnapshotTypeName",
                table: "LessonPreparations");

            migrationBuilder.DropColumn(
                name: "SnapshotClassName",
                table: "LessonPlanners");

            migrationBuilder.DropColumn(
                name: "SnapshotGradeLevelName",
                table: "LessonPlanners");

            migrationBuilder.DropColumn(
                name: "SnapshotMethodDescription",
                table: "LessonPlanners");

            migrationBuilder.DropColumn(
                name: "SnapshotMethodName",
                table: "LessonPlanners");

            migrationBuilder.DropColumn(
                name: "SnapshotContent",
                table: "LessonObjectives");

            migrationBuilder.DropColumn(
                name: "SnapshotName",
                table: "LessonObjectives");

            migrationBuilder.DropColumn(
                name: "SnapshotTypeName",
                table: "LessonLanguageFocusItems");

            migrationBuilder.DropColumn(
                name: "SnapshotContent",
                table: "LessonAttitudes");

            migrationBuilder.DropColumn(
                name: "SnapshotName",
                table: "LessonAttitudes");

            migrationBuilder.DropColumn(
                name: "SnapshotInteractionName",
                table: "LessonActivityItems");

            migrationBuilder.DropColumn(
                name: "SnapshotInteractionShortCode",
                table: "LessonActivityItems");

            migrationBuilder.AlterColumn<int>(
                name: "PreparationTypeId",
                table: "LessonPreparations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations",
                column: "PreparationTypeId",
                principalTable: "PreparationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
