using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class ClassResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GradeLevelId { get; set; }
        public string GradeLevelName { get; set; }
    }
}
