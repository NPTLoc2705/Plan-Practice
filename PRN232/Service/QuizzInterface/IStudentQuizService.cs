using BusinessObject.Dtos.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IStudentQuizService
    {
        Task<IEnumerable<QuizInfoDto>> GetAvailableQuizzesAsync(int userId);
        Task<QuizDetailDto> GetQuizForTakingAsync(int quizId, int userId);
        Task<QuizResultDto> SubmitQuizAsync(int userId, SubmitQuizDto dto);
        Task<QuizResultDto> GetQuizResultAsync(int resultId, int userId);
        Task<IEnumerable<QuizHistoryDto>> GetUserQuizHistoryAsync(int userId);
        Task<QuizStatisticsDto> GetQuizStatisticsAsync(int quizId, int userId);
        Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId);
    }
}
