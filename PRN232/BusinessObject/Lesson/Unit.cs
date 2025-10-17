using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class Unit
    {
        public int Id { get; set; }

        public int GradeLevelId { get; set; }
        [ForeignKey("GradeLevelId")]
        public virtual GradeLevel GradeLevel { get; set; }

        // Inherits UserId through GradeLevel relationship (no direct FK needed)

        [Required]
        [MaxLength(100)]
        public string UnitNumber { get; set; } // e.g., "Unit 6"

        [Required]
        [MaxLength(200)]
        public string UnitName { get; set; } // e.g., "Endangered Species"

        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation
        public virtual ICollection<LessonDefinition> LessonDefinitions { get; set; }
        public virtual ICollection<LessonPlanner> LessonPlanners { get; set; }
    }

}
