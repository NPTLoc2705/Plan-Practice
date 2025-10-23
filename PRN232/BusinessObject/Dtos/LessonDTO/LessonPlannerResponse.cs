using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonPlannerResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfPreparation { get; set; }
        public DateTime? DateOfTeaching { get; set; }
        public string LessonNumber { get; set; }
        public string UnitNumber { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? LessonDefinitionId { get; set; }
        public string LessonDefinitionTitle { get; set; }
        public int? MethodTemplateId { get; set; }
        public string MethodTemplateName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<LessonObjectiveDto> Objectives { get; set; } = new();
        public List<LessonSkillDto> Skills { get; set; } = new();
        public List<LessonAttitudeDto> Attitudes { get; set; } = new();
        public List<LessonLanguageFocusDto> LanguageFocusItems { get; set; } = new();
        public List<LessonPreparationDto> Preparations { get; set; } = new();
        public List<LessonActivityStageDto> ActivityStages { get; set; } = new();
    }
}
