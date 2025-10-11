using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IAnswerStudentRepository
    {
        Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId);
        Task<Answer> GetCorrectAnswerByQuestionIdAsync(int questionId);
        Task<bool> IsAnswerCorrectAsync(int answerId);
    }
}