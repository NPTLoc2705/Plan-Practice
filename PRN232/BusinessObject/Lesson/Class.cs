using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class Class
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "6A", "10B"

        public int GradeLevelId { get; set; }
        [ForeignKey("GradeLevelId")]
        public virtual GradeLevel GradeLevel { get; set; }

        // Inherits UserId through GradeLevel relationship (no direct FK needed)

        // Navigation
        public virtual ICollection<LessonPlanner> LessonPlanners { get; set; }
    }
}
