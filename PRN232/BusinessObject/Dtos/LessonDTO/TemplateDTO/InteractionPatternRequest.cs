using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO.TemplateDTO
{
    public class InteractionPatternRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Individual Work", "Pair Work"

        [MaxLength(50)]
        public string ShortCode { get; set; } // e.g., "IW", "PW"

        public string Description { get; set; }
    }
}
