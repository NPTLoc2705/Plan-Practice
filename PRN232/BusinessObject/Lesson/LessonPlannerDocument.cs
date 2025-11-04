using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Lesson
{
    public class LessonPlannerDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        public int LessonPlannerId { get; set; }
        
        [ForeignKey("LessonPlannerId")]
        public virtual LessonPlanner LessonPlanner { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
