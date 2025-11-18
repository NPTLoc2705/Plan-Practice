using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Dtos.LessonDTO
{
    /// <summary>
    /// Request DTO for AI-generated lesson planner
    /// </summary>
    public class GenerateLessonPlannerRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        public int ClassId { get; set; }

        public string? GradeLevel { get; set; }

        public int? DurationInMinutes { get; set; } = 45;

        public string? LearningObjectives { get; set; }

        public string? AdditionalInstructions { get; set; }
    }

    /// <summary>
    /// Response from AI lesson generation
    /// </summary>
    public class GeneratedLessonPlannerResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string LessonNumber { get; set; }
        public string UnitNumber { get; set; }
        public string UnitName { get; set; }
        public string MethodName { get; set; }
        public string MethodDescription { get; set; }
        
        public List<GeneratedObjective> Objectives { get; set; } = new();
        public List<GeneratedSkill> Skills { get; set; } = new();
        public List<GeneratedAttitude> Attitudes { get; set; } = new();
        public List<GeneratedLanguageFocus> LanguageFocusItems { get; set; } = new();
        public List<GeneratedPreparation> Preparations { get; set; } = new();
        public List<GeneratedActivityStage> ActivityStages { get; set; } = new();
    }

    public class GeneratedObjective
    {
        public string Name { get; set; }
        public string CustomContent { get; set; }
    }

    public class GeneratedSkill
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class GeneratedAttitude
    {
        public string Name { get; set; }
        public string CustomContent { get; set; }
    }

    public class GeneratedLanguageFocus
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class GeneratedPreparation
    {
        public string Name { get; set; }
        public string Materials { get; set; }
    }

    public class GeneratedActivityStage
    {
        public string StageName { get; set; }
        public List<GeneratedActivityItem> ActivityItems { get; set; } = new();
    }

    public class GeneratedActivityItem
    {
        public int TimeInMinutes { get; set; }
        public string InteractionPatternName { get; set; }
        public string InteractionPatternShortCode { get; set; }
        public string ActivityTemplateName { get; set; }
        public string ActivityTemplateContent { get; set; }
    }
}
