using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO
{
    public class ClassRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "6A", "10B"
        [Required]
        public int GradeLevelId { get; set; }
    }
}
