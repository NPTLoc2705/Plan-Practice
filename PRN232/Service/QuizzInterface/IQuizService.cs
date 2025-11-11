using BusinessObject.Dtos;
using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IQuizService
    {
        Task<IEnumerable<Quizs>> GetAllQuizzesAsync();
        Task<Quizs?> GetQuizByIdAsync(int quizId);
        Task CreateQuizAsync(Quizs quiz);
        Task UpdateQuizAsync(Quizs quiz);
        Task DeleteQuizAsync(int quizId);
        Task<TeacherDashboardDto> GetTeacherDashboardStatsAsync(int teacherId);
        Task<IEnumerable<Quizs>> GetQuizzesByTeacherAsync(int teacherId);

        //=========================AI====================================//

        Task<Quizs> CreateQuizWithAIAsync(int lessonPlannerId, string title, string description, int numberOfQuestions);


    }
}