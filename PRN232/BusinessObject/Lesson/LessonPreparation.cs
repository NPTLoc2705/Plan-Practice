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
    public class LessonPreparation
    {
        public int Id { get; set; }

        public int LessonPlannerId { get; set; }
        [ForeignKey("LessonPlannerId")]
        public virtual LessonPlanner LessonPlanner { get; set; }

        public int PreparationTypeId { get; set; }
        [ForeignKey("PreparationTypeId")]
        public virtual PreparationType PreparationType { get; set; }

        [Required]
        public string Materials { get; set; }
        public int DisplayOrder { get; set; }
    }
}
