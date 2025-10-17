using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class LessonDefinition
    {
        public int Id { get; set; }

        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        // Inherits UserId through Unit relationship (no direct FK needed)

        [Required]
        [MaxLength(100)]
        public string LessonNumber { get; set; } // e.g. "Lesson 1", "Getting Started"

        [Required]
        [MaxLength(200)]
        public string LessonTitle { get; set; }

        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation
        public virtual ICollection<LessonPlanner> LessonPlanners { get; set; }
    }
}
