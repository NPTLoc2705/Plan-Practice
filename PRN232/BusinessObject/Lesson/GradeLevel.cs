using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class GradeLevel
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Grade 1"
        public int Level { get; set; }   // e.g., 1
        public virtual ICollection<Class> Classes { get; set; }
    }
}
