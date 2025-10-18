using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BusinessObject.Lesson.Template;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class LessonPlanner
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Content { get; set; }
        public string? Description { get; set; }

        // Metadata
        public DateTime? DateOfPreparation { get; set; }
        public DateTime? DateOfTeaching { get; set; }

        [MaxLength(100)]
        public string? LessonNumber { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        // Link to Unit and predefined Lesson Definition
        public int? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        public int? LessonDefinitionId { get; set; }
        [ForeignKey("LessonDefinitionId")]
        public virtual LessonDefinition LessonDefinition { get; set; }

        public int? MethodTemplateId { get; set; }
        [ForeignKey("MethodTemplateId")]
        public virtual MethodTemplate MethodTemplate { get; set; }

        // Navigation Properties
        public virtual ICollection<LessonObjective> Objectives { get; set; }
        public virtual ICollection<LessonSkill> Skills { get; set; }
        public virtual ICollection<LessonAttitude> Attitudes { get; set; }
        public virtual ICollection<LessonLanguageFocus> LanguageFocusItems { get; set; }
        public virtual ICollection<LessonPreparation> Preparations { get; set; }
        public virtual ICollection<LessonActivityStage> ActivityStages { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // History/Snapshot fields for Class and GradeLevel
        public string? SnapshotClassName { get; set; }
        public string? SnapshotGradeLevelName { get; set; }

        // History/Snapshot fields for Method
        public string? SnapshotMethodName { get; set; }
        public string? SnapshotMethodDescription { get; set; }
    }
}
