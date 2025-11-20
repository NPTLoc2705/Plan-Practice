using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class moreNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlanners_MethodTemplates_MethodTemplateId",
                table: "LessonPlanners");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlanners_Users_UserId",
                table: "LessonPlanners");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlanners_MethodTemplates_MethodTemplateId",
                table: "LessonPlanners",
                column: "MethodTemplateId",
                principalTable: "MethodTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlanners_Users_UserId",
                table: "LessonPlanners",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId",
                principalTable: "LessonPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlanners_MethodTemplates_MethodTemplateId",
                table: "LessonPlanners");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlanners_Users_UserId",
                table: "LessonPlanners");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlanners_MethodTemplates_MethodTemplateId",
                table: "LessonPlanners",
                column: "MethodTemplateId",
                principalTable: "MethodTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlanners_Users_UserId",
                table: "LessonPlanners",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LessonPlanners_LessonPlannerId",
                table: "Quizzes",
                column: "LessonPlannerId",
                principalTable: "LessonPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
