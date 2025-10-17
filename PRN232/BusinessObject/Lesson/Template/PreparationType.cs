using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson.Template
{
    public class PreparationType
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Materials", "Technology", "Room Setup"

        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
