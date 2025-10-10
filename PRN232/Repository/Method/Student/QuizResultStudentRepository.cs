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
    public class QuizResultStudentRepository : IQuizResultStudentRepository
    {

        private readonly QuizResultStudentDAO quizResultStudentDAO;

        public QuizResultStudentRepository(QuizResultStudentDAO quizResultStudentDAO)
        {
            this.quizResultStudentDAO = quizResultStudentDAO;
        }



        public Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId) => quizResultStudentDAO.GetQuizResultsByQuizIdAsync(quizId);






        public Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId) => quizResultStudentDAO.HasUserAttemptedQuizAsync(userId, quizId);



        public Task<QuizResult> GetLatestQuizResultByUserAndQuizAsync(int userId, int quizId) => quizResultStudentDAO.GetLatestQuizResultByUserAndQuizAsync(userId, quizId);




        public Task<QuizResult> GetQuizResultWithDetailsAsync(int quizResultId)
       => quizResultStudentDAO.GetQuizResultWithDetailsAsync(quizResultId);

        public Task<IEnumerable<QuizResult>> GetCompletedQuizResultsByUserAsync(int userId)
        => quizResultStudentDAO.GetCompletedQuizResultsByUserAsync(userId);

        public Task<QuizResult> GetInProgressQuizResultAsync(int userId, int quizId)
        => quizResultStudentDAO.GetInProgressQuizResultAsync(userId, quizId);

    }
}

