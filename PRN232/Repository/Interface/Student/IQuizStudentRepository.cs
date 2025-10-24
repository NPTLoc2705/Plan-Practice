using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IQuizStudentRepository
    {


        Task<Quizs> GetQuizWithQuestionsAndAnswersAsync(int quizId);

    }
}
