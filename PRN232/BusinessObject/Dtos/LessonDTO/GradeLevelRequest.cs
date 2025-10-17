using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class GradeLevelRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Grade 6", "Grade 10"
        [Required]
        public int Level { get; set; } // Numeric level for sorting
    }
}
