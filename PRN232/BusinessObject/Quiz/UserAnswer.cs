using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Quiz
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }

        public int QuizResultId { get; set; }

        public int QuestionId { get; set; }

        public int AnswerId { get; set; }

        [ForeignKey("QuizResultId")]
        public virtual QuizResult QuizResult { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("AnswerId")]
        public virtual Answer Answer { get; set; }
    }
}
