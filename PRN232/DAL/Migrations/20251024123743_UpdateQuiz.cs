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
                name: "FK_Quizzes_Users_CreatedBy",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CreatedBy",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "LessonPlannerId",
                table: "Quizzes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId",
                principalTable: "LessonPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Quizzes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CreatedBy",
                table: "Quizzes",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Users_CreatedBy",
                table: "Quizzes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
