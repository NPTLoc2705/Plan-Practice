using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SetNullOnDeleteForLessonPlannerTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonActivityItems_ActivityTemplates_ActivityTemplateId",
                table: "LessonActivityItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonActivityItems_InteractionPatterns_InteractionPatternId",
                table: "LessonActivityItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonAttitudes_AttitudeTemplates_AttitudeTemplateId",
                table: "LessonAttitudes");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLanguageFocusItems_LanguageFocusTypes_LanguageFocusTy~",
                table: "LessonLanguageFocusItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonObjectives_ObjectiveTemplates_ObjectiveTemplateId",
                table: "LessonObjectives");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSkills_SkillTemplates_SkillTemplateId",
                table: "LessonSkills");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonActivityItems_ActivityTemplates_ActivityTemplateId",
                table: "LessonActivityItems",
                column: "ActivityTemplateId",
                principalTable: "ActivityTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonActivityItems_InteractionPatterns_InteractionPatternId",
                table: "LessonActivityItems",
                column: "InteractionPatternId",
                principalTable: "InteractionPatterns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonAttitudes_AttitudeTemplates_AttitudeTemplateId",
                table: "LessonAttitudes",
                column: "AttitudeTemplateId",
                principalTable: "AttitudeTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonLanguageFocusItems_LanguageFocusTypes_LanguageFocusTy~",
                table: "LessonLanguageFocusItems",
                column: "LanguageFocusTypeId",
                principalTable: "LanguageFocusTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonObjectives_ObjectiveTemplates_ObjectiveTemplateId",
                table: "LessonObjectives",
                column: "ObjectiveTemplateId",
                principalTable: "ObjectiveTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations",
                column: "PreparationTypeId",
                principalTable: "PreparationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSkills_SkillTemplates_SkillTemplateId",
                table: "LessonSkills",
                column: "SkillTemplateId",
                principalTable: "SkillTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonActivityItems_ActivityTemplates_ActivityTemplateId",
                table: "LessonActivityItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonActivityItems_InteractionPatterns_InteractionPatternId",
                table: "LessonActivityItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonAttitudes_AttitudeTemplates_AttitudeTemplateId",
                table: "LessonAttitudes");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLanguageFocusItems_LanguageFocusTypes_LanguageFocusTy~",
                table: "LessonLanguageFocusItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonObjectives_ObjectiveTemplates_ObjectiveTemplateId",
                table: "LessonObjectives");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSkills_SkillTemplates_SkillTemplateId",
                table: "LessonSkills");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonActivityItems_ActivityTemplates_ActivityTemplateId",
                table: "LessonActivityItems",
                column: "ActivityTemplateId",
                principalTable: "ActivityTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonActivityItems_InteractionPatterns_InteractionPatternId",
                table: "LessonActivityItems",
                column: "InteractionPatternId",
                principalTable: "InteractionPatterns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonAttitudes_AttitudeTemplates_AttitudeTemplateId",
                table: "LessonAttitudes",
                column: "AttitudeTemplateId",
                principalTable: "AttitudeTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonLanguageFocusItems_LanguageFocusTypes_LanguageFocusTy~",
                table: "LessonLanguageFocusItems",
                column: "LanguageFocusTypeId",
                principalTable: "LanguageFocusTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonObjectives_ObjectiveTemplates_ObjectiveTemplateId",
                table: "LessonObjectives",
                column: "ObjectiveTemplateId",
                principalTable: "ObjectiveTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPreparations_PreparationTypes_PreparationTypeId",
                table: "LessonPreparations",
                column: "PreparationTypeId",
                principalTable: "PreparationTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSkills_SkillTemplates_SkillTemplateId",
                table: "LessonSkills",
                column: "SkillTemplateId",
                principalTable: "SkillTemplates",
                principalColumn: "Id");
        }
    }
}
