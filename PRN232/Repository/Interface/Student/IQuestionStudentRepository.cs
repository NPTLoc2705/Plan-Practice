using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IQuestionStudentRepository
    {
        Task<int> GetQuestionCountByQuizIdAsync(int quizId);
    }
}
