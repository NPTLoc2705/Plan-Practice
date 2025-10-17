using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO.TemplateDTO
{
    public class SkillTemplateRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SkillTypeId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }
    }
}
