using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Quiz
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}