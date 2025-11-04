using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.AlterColumn<int>(
                name: "LessonPlannerId",
                table: "Quizzes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId",
                principalTable: "LessonPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.AlterColumn<int>(
                name: "LessonPlannerId",
                table: "Quizzes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId",
                principalTable: "LessonPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
