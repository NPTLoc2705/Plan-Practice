using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.LessonDTO.TemplateDTO
{
    public class LanguageFocusTypeResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
