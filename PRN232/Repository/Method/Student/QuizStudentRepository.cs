using BusinessObject.Quiz;
using DAL.Student;
using Repository.Interface.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Student
{
    public class QuizStudentRepository : IQuizStudentRepository
    {
        private readonly QuizStudentDAO quizStudentDAO;

        public QuizStudentRepository(QuizStudentDAO quizStudentDAO)
        {
            this.quizStudentDAO = quizStudentDAO;
        }
        public Task<Quiz> GetQuizWithQuestionsAndAnswersAsync(int quizId) => quizStudentDAO.GetQuizWithQuestionsAndAnswersAsync(quizId);




    }
}