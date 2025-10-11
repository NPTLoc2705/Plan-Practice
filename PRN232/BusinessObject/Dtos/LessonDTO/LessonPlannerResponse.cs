using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonPlannerResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }

        // Detailed relationship info
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int GradeLevelId { get; set; }
        public string GradeLevelName { get; set; }
    }
}
