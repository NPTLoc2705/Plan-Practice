using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson.Template
{
    public class LanguageFocusType
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Vocabulary", "Grammar", "Pronunciation"

        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
