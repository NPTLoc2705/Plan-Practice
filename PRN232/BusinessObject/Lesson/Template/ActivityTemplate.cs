using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson.Template
{
    public class ActivityTemplate
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } // e.g., "Listen and Read", "Group Discussion", "Vocabulary Practice"

        [Required]
        public string Content { get; set; } // The actual activity content/instructions

        public DateTime CreatedAt { get; set; }
    }
}
