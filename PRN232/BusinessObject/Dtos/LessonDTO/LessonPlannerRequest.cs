using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonPlannerRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        public string? Content { get; set; }
        public string? Description { get; set; }
        public DateTime? DateOfPreparation { get; set; }
        public DateTime? DateOfTeaching { get; set; }

        [MaxLength(100)]
        public string? LessonNumber { get; set; }

        [MaxLength(100)]
        public string? UnitNumber { get; set; }

        [MaxLength(200)]
        public string? UnitName { get; set; }

        [Required]
        public int ClassId { get; set; }
        
        public int? UnitId { get; set; }
        public int? LessonDefinitionId { get; set; }
        public int? MethodTemplateId { get; set; }

        [JsonPropertyName("objectives")]
        public List<LessonObjectiveDto>? Objectives { get; set; } = new();
        [JsonPropertyName("skills")]
        public List<LessonSkillDto>? Skills { get; set; } = new();
        [JsonPropertyName("attitudes")]
        public List<LessonAttitudeDto>? Attitudes { get; set; } = new();
        [JsonPropertyName("languageFocusItems")]
        public List<LessonLanguageFocusDto>? LanguageFocusItems { get; set; } = new();
        [JsonPropertyName("preparations")]
        public List<LessonPreparationDto>? Preparations { get; set; } = new();
        [JsonPropertyName("activityStages")]
        public List<LessonActivityStageDto>? ActivityStages { get; set; } = new();
    }


    public class LessonObjectiveDto 
    { 
        public int Id { get; set; } 
        public int? ObjectiveTemplateId { get; set; } 
        public string? CustomContent { get; set; } 
        public int DisplayOrder { get; set; } 
    }
    
    public class LessonSkillDto 
    { 
        public int Id { get; set; } 
        public int? SkillTemplateId { get; set; } 
        public string? Description { get; set; } 
        public int DisplayOrder { get; set; } 
    }
    
    public class LessonAttitudeDto 
    { 
        public int Id { get; set; } 
        public int? AttitudeTemplateId { get; set; } 
        public string? CustomContent { get; set; } 
        public int DisplayOrder { get; set; } 
    }
    
    public class LessonLanguageFocusDto 
    { 
        public int Id { get; set; } 
        public int? LanguageFocusTypeId { get; set; } 
        public string? Content { get; set; } 
        public int DisplayOrder { get; set; } 
    }
    
    public class LessonPreparationDto 
    { 
        public int Id { get; set; } 
        public int? PreparationTypeId { get; set; } 
        public string? Materials { get; set; } 
        public int DisplayOrder { get; set; } 
    }
    
    public class LessonActivityItemDto 
    { 
        public int Id { get; set; } 
        public int TimeInMinutes { get; set; } 
        public string? Content { get; set; } 
        public int? InteractionPatternId { get; set; }
        public int? ActivityTemplateId { get; set; }
        public int DisplayOrder { get; set; } 
    }
    
    public class LessonActivityStageDto 
    { 
        public int Id { get; set; } 
        public string? StageName { get; set; } 
        public int DisplayOrder { get; set; }
        [JsonPropertyName("activityItems")]
        public List<LessonActivityItemDto>? ActivityItems { get; set; } = new(); 
    }
}
