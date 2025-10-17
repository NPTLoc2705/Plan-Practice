using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Lesson
{
    public class GradeLevel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Grade 6", "Grade 10"

        public int Level { get; set; } // Numeric level for sorting

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Navigation
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
    }
}
