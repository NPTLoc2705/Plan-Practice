using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Lesson.Template;

namespace BusinessObject.Lesson
{
    public class LessonSkill
    {
        public int Id { get; set; }

        public int LessonPlannerId { get; set; }
        [ForeignKey("LessonPlannerId")]
        public virtual LessonPlanner LessonPlanner { get; set; }

        public int? SkillTemplateId { get; set; }
        [ForeignKey("SkillTemplateId")]
        public virtual SkillTemplate SkillTemplate { get; set; }

        public string? CustomContent { get; set; }
        public int DisplayOrder { get; set; }

        // History/Snapshot fields - preserve data even if template is deleted
        public string? SnapshotSkillType { get; set; }
        public string? SnapshotName { get; set; }
        public string? SnapshotDescription { get; set; }
    }
}
