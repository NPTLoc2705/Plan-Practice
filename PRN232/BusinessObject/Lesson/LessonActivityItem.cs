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

        public string? Content { get; set; }

        public int? InteractionPatternId { get; set; }
        [ForeignKey("InteractionPatternId")]
        public virtual InteractionPattern InteractionPattern { get; set; }

        public int? ActivityTemplateId { get; set; }
        [ForeignKey("ActivityTemplateId")]
        public virtual ActivityTemplate ActivityTemplate { get; set; }

        public int DisplayOrder { get; set; }

        // History/Snapshot fields - preserve data even if interaction pattern is deleted
        public string? SnapshotInteractionName { get; set; }
        public string? SnapshotInteractionShortCode { get; set; }

        // History/Snapshot fields - preserve data even if activity template is deleted
        public string? SnapshotActivityName { get; set; }
        public string? SnapshotActivityDescription { get; set; }
        public string? SnapshotActivityContent { get; set; }
    }
}
