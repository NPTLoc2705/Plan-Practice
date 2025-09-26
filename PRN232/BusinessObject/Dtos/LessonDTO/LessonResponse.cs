using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string GradeLevel { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
    }
}
