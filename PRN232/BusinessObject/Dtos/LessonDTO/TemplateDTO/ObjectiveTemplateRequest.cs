using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO.TemplateDTO
{
    public class ObjectiveTemplateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
    }
}
