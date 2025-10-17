using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Lesson.Template;

namespace BusinessObject.Lesson
{
    public class LessonActivityItem
    {
        public int Id { get; set; }

        public int LessonActivityStageId { get; set; }
        [ForeignKey("LessonActivityStageId")]
        public virtual LessonActivityStage LessonActivityStage { get; set; }

        public int TimeInMinutes { get; set; }

        [Required]
        public string Content { get; set; }

        public int? InteractionPatternId { get; set; }
        [ForeignKey("InteractionPatternId")]
        public virtual InteractionPattern InteractionPattern { get; set; }

        public int DisplayOrder { get; set; }
    }
}
