using DAL.Student;
using Repository.Interface.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Student
{
    public class QuestionStudentRepository : IQuestionStudentRepository
    {
        private readonly QuestionStudentDAO _questionStudentDAO;
        public QuestionStudentRepository(QuestionStudentDAO questionStudentDAO)
        {
            this._questionStudentDAO = questionStudentDAO;
        }
        public Task<int> GetQuestionCountByQuizIdAsync(int quizId) => _questionStudentDAO.GetQuestionCountByQuizIdAsync(quizId);





    }
}