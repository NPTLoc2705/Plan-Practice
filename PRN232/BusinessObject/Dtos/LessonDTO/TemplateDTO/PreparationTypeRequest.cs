using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO.TemplateDTO
{
    public class PreparationTypeRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Materials", "Technology", "Room Setup"

        public string Description { get; set; }
    }

}
