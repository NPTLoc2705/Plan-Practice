using DAL.Student;
using Repository.Interface.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Student
{
    public class QuizStatisticsStudentRepository : IQuizStatisticsStudentRepository
    {
        private readonly QuizStatisticsStudentDAO quizStatisticsStudentDAO;

        public QuizStatisticsStudentRepository(QuizStatisticsStudentDAO quizStatisticsStudentDAO)
        {
            this.quizStatisticsStudentDAO = quizStatisticsStudentDAO;
        }

        public Task<int> GetUserAttemptCountAsync(int userId, int quizId)
       => quizStatisticsStudentDAO.GetUserAttemptCountAsync(userId, quizId);

        public Task<int> GetUserBestScoreAsync(int userId, int quizId)
       => quizStatisticsStudentDAO.GetUserBestScoreAsync(userId, quizId);
    }
}
