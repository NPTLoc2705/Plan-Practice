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
    public class AnswerStudentRepository : IAnswerStudentRepository
    {
        private readonly AnswerStudentDAO _answerStudentDAO;
        public AnswerStudentRepository(AnswerStudentDAO answerStudentDAO)
        {
            _answerStudentDAO = answerStudentDAO;
        }
        public Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId) => _answerStudentDAO.GetAnswersByQuestionIdAsync(questionId);


        public Task<Answer> GetCorrectAnswerByQuestionIdAsync(int questionId)
       => _answerStudentDAO.GetCorrectAnswerByQuestionIdAsync(questionId);




        public Task<bool> IsAnswerCorrectAsync(int answerId) => _answerStudentDAO.IsAnswerCorrectAsync(answerId);

    }
}