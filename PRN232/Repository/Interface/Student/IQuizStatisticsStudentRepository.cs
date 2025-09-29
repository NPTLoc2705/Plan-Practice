using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IQuizStatisticsStudentRepository
    {
        Task<int> GetUserBestScoreAsync(int userId, int quizId);
        Task<int> GetUserAttemptCountAsync(int userId, int quizId);
    }
}
