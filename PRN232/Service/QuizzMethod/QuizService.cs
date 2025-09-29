using BusinessObject.Dtos;
using BusinessObject.Quiz;
using Repository.Interface;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _quizRepository.GetTotalQuizzesAsync();
        }

        public async Task<Quiz?> GetQuizByIdAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));

            return await _quizRepository.GetQuizByIdAsync(quizId);
        }

        public async Task CreateQuizAsync(Quiz quiz)
        {
            if (quiz == null)
                throw new ArgumentNullException(nameof(quiz));

            if (string.IsNullOrWhiteSpace(quiz.Title))
                throw new ArgumentException("Quiz title is required", nameof(quiz));

            await _quizRepository.CreateQuizAsync(quiz);
           
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {  
            await _quizRepository.UpdateQuizAsync(quiz);           
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));
            await _quizRepository.DeleteQuizAsync(quizId);
        }

        public async Task<TeacherDashboardDto> GetTeacherDashboardStatsAsync(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentException("Teacher ID must be greater than 0", nameof(teacherId));

            var teacherQuizzes = await _quizRepository.GetQuizzesByTeacherAsync(teacherId);
            var allQuizResults = new List<QuizResult>();
            
            foreach (var quiz in teacherQuizzes)
            {
                var results = await _quizRepository.GetQuizResultsByQuizIdAsync(quiz.Id);
                allQuizResults.AddRange(results);
            }

            var totalTestsCreated = teacherQuizzes.Count();
            var totalTestsTaken = allQuizResults.Count();
            var averageScore = allQuizResults.Any() ? allQuizResults.Average(r => r.Score) : 0;

            var recentQuizzes = teacherQuizzes
                .OrderByDescending(q => q.CreatedAt)
                .Take(5)
                .Select(q => new QuizStatDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    CreatedAt = q.CreatedAt,
                    TimesTaken = q.QuizResults?.Count ?? 0,
                    AverageScore = q.QuizResults?.Any() == true ? q.QuizResults.Average(r => r.Score) : 0
                }).ToList();

            return new TeacherDashboardDto
            {
                TotalTestsCreated = totalTestsCreated,
                TotalTestsTaken = totalTestsTaken,
                AverageScore = averageScore,
                RecentQuizzes = recentQuizzes
            };
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByTeacherAsync(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentException("Teacher ID must be greater than 0", nameof(teacherId));

            return await _quizRepository.GetQuizzesByTeacherAsync(teacherId);
        }
    }
}