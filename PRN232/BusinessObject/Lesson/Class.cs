using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Class 7A"
        public int GradeLevelId { get; set; }
        [ForeignKey("GradeLevelId")]
        public virtual GradeLevel GradeLevel { get; set; }
        public virtual ICollection<LessonPlanner> LessonPlanners { get; set; }
    }
}
