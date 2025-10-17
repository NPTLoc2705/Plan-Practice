using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{

    public class LessonObjective
    {
        public int Id { get; set; }

        public int LessonPlannerId { get; set; }
        [ForeignKey("LessonPlannerId")]
        public virtual LessonPlanner LessonPlanner { get; set; }

        public int? ObjectiveTemplateId { get; set; }
        [ForeignKey("ObjectiveTemplateId")]
        public virtual ObjectiveTemplate ObjectiveTemplate { get; set; }

        public string CustomContent { get; set; } // Override template if needed
        public int DisplayOrder { get; set; }
    }
}
