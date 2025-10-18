using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class LessonAttitude
    {
        public int Id { get; set; }

        public int LessonPlannerId { get; set; }
        [ForeignKey("LessonPlannerId")]
        public virtual LessonPlanner LessonPlanner { get; set; }

        public int? AttitudeTemplateId { get; set; }
        [ForeignKey("AttitudeTemplateId")]
        public virtual AttitudeTemplate AttitudeTemplate { get; set; }

        public string? CustomContent { get; set; }
        public int DisplayOrder { get; set; }

        // History/Snapshot fields - preserve data even if template is deleted
        public string? SnapshotName { get; set; }
        public string? SnapshotContent { get; set; }
    }
}
