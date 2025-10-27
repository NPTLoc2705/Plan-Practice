using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    /// <summary>
    /// Extension methods to retrieve historical/snapshot data from LessonPlanner entities
    /// Falls back to snapshot data when referenced entities are deleted
    /// </summary>
    public static class LessonPlannerExtensions
    {
        /// <summary>
        /// Get the class name, using snapshot if the Class entity is deleted
        /// </summary>
        public static string GetClassName(this LessonPlanner lessonPlanner)
        {
            return lessonPlanner.Class?.Name ?? lessonPlanner.SnapshotClassName ?? "Unknown Class";
        }

        /// <summary>
        /// Get the grade level name, using snapshot if the GradeLevel entity is deleted
        /// </summary>
        public static string GetGradeLevelName(this LessonPlanner lessonPlanner)
        {
            return lessonPlanner.Class?.GradeLevel?.Name ?? lessonPlanner.SnapshotGradeLevelName ?? "Unknown Grade";
        }

        /// <summary>
        /// Get the method template name, using snapshot if the MethodTemplate is deleted
        /// </summary>
        public static string GetMethodName(this LessonPlanner lessonPlanner)
        {
            return lessonPlanner.MethodTemplate?.Name ?? lessonPlanner.SnapshotMethodName ?? string.Empty;
        }

        /// <summary>
        /// Get the method template description, using snapshot if the MethodTemplate is deleted
        /// </summary>
        public static string GetMethodDescription(this LessonPlanner lessonPlanner)
        {
            return lessonPlanner.MethodTemplate?.Description ?? lessonPlanner.SnapshotMethodDescription ?? string.Empty;
        }

        /// <summary>
        /// Get the objective name, using snapshot if the ObjectiveTemplate is deleted
        /// </summary>
        public static string GetObjectiveName(this LessonObjective lessonObjective)
        {
            return lessonObjective.ObjectiveTemplate?.Name ?? lessonObjective.SnapshotName ?? "Unknown Objective";
        }

        /// <summary>
        /// Get the objective content, using snapshot if the ObjectiveTemplate is deleted
        /// </summary>
        public static string GetObjectiveContent(this LessonObjective lessonObjective)
        {
            return lessonObjective.ObjectiveTemplate?.Content ?? lessonObjective.SnapshotContent ?? string.Empty;
        }

        /// <summary>
        /// Get the skill name, using snapshot if the SkillTemplate is deleted
        /// </summary>
        public static string GetSkillName(this LessonSkill lessonSkill)
        {
            return lessonSkill.SkillTemplate?.Name ?? lessonSkill.SnapshotName ?? "Unknown Skill";
        }

        /// <summary>
        /// Get the skill description, using snapshot if the SkillTemplate is deleted
        /// </summary>
        public static string GetSkillDescription(this LessonSkill lessonSkill)
        {
            return lessonSkill.SkillTemplate?.Description ?? lessonSkill.SnapshotDescription ?? string.Empty;
        }

        /// <summary>
        /// Get the attitude name, using snapshot if the AttitudeTemplate is deleted
        /// </summary>
        public static string GetAttitudeName(this LessonAttitude lessonAttitude)
        {
            return lessonAttitude.AttitudeTemplate?.Name ?? lessonAttitude.SnapshotName ?? "Unknown Attitude";
        }

        /// <summary>
        /// Get the attitude content, using snapshot if the AttitudeTemplate is deleted
        /// </summary>
        public static string GetAttitudeContent(this LessonAttitude lessonAttitude)
        {
            return lessonAttitude.AttitudeTemplate?.Content ?? lessonAttitude.SnapshotContent ?? string.Empty;
        }

        /// <summary>
        /// Get the language focus type name, using snapshot if the LanguageFocusType is deleted
        /// </summary>
        public static string GetLanguageFocusTypeName(this LessonLanguageFocus lessonLanguageFocus)
        {
            return lessonLanguageFocus.LanguageFocusType?.Name ?? lessonLanguageFocus.SnapshotTypeName ?? "Unknown Type";
        }

        /// <summary>
        /// Get the preparation type name, using snapshot if the PreparationType is deleted
        /// </summary>
        public static string GetPreparationTypeName(this LessonPreparation lessonPreparation)
        {
            return lessonPreparation.PreparationType?.Name ?? lessonPreparation.SnapshotTypeName ?? "Unknown Type";
        }

        /// <summary>
        /// Get the interaction pattern name, using snapshot if the InteractionPattern is deleted
        /// </summary>
        public static string GetInteractionPatternName(this LessonActivityItem activityItem)
        {
            return activityItem.InteractionPattern?.Name ?? activityItem.SnapshotInteractionName ?? string.Empty;
        }

        /// <summary>
        /// Get the interaction pattern short code, using snapshot if the InteractionPattern is deleted
        /// </summary>
        public static string GetInteractionPatternShortCode(this LessonActivityItem activityItem)
        {
            return activityItem.InteractionPattern?.ShortCode ?? activityItem.SnapshotInteractionShortCode ?? string.Empty;
        }
    }
}
