using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class LessonActivityStage
    {
        public int Id { get; set; }

        public int LessonPlannerId { get; set; }
        [ForeignKey("LessonPlannerId")]
        public virtual LessonPlanner LessonPlanner { get; set; }

        [Required]
        [MaxLength(100)]
        public string StageName { get; set; } // e.g., "Warm-up", "Presentation", "Practice"

        public int DisplayOrder { get; set; }

        // Navigation
        public virtual ICollection<LessonActivityItem> ActivityItems { get; set; }
    }
}
