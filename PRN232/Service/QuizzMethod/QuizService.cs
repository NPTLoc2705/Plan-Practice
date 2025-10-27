using BusinessObject.Dtos;
using BusinessObject.Quiz;
using Repository.Interface;
using Service.QuizzInterface;
using Service.Interface;
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
        private readonly ILessonPlannerService _lessonPlannerService;

        public QuizService(IQuizRepository quizRepository, ILessonPlannerService lessonPlannerService)
        {
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
            _lessonPlannerService = lessonPlannerService ?? throw new ArgumentNullException(nameof(lessonPlannerService));
        }

        public async Task<IEnumerable<Quizs>> GetAllQuizzesAsync()
        {
            return await _quizRepository.GetTotalQuizzesAsync();
        }

        public async Task<Quizs?> GetQuizByIdAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));

            return await _quizRepository.GetQuizByIdAsync(quizId);
        }

        public async Task CreateQuizAsync(Quizs quiz)
        {
            if (quiz == null)
                throw new ArgumentNullException(nameof(quiz));

            if (string.IsNullOrWhiteSpace(quiz.Title))
                throw new ArgumentException("Quiz title is required", nameof(quiz));

            await _quizRepository.CreateQuizAsync(quiz);
           
        }

        public async Task UpdateQuizAsync(Quizs quiz)
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

            // Get Quiz Statistics
            var teacherQuizzes = await _quizRepository.GetQuizzesByTeacherAsync(teacherId);
            var allQuizResults = new List<QuizResult>();
            
            foreach (var quiz in teacherQuizzes)
            {
                var results = await _quizRepository.GetQuizResultsByQuizIdAsync(quiz.Id);
                allQuizResults.AddRange(results);
            }

            var totalQuizzesCreated = teacherQuizzes.Count();
            var totalQuizzesTaken = allQuizResults.Count();
            var averageQuizScore = allQuizResults.Any() ? allQuizResults.Average(r => r.Score) : 0;

            // Get unique students who took quizzes
            var uniqueStudents = allQuizResults.Select(r => r.UserId).Distinct().Count();

            // Calculate quiz performance breakdown
            var quizPerformance = new QuizPerformanceBreakdown
            {
                ExcellentCount = allQuizResults.Count(r => r.Score >= 90),
                GoodCount = allQuizResults.Count(r => r.Score >= 70 && r.Score < 90),
                AverageCount = allQuizResults.Count(r => r.Score >= 50 && r.Score < 70),
                BelowAverageCount = allQuizResults.Count(r => r.Score < 50)
            };

            // Get recent quizzes
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

            // Get Lesson Statistics
            var teacherLessons = await _lessonPlannerService.GetLessonPlannersByUserIdAsync(teacherId);
            var totalLessonsCreated = teacherLessons.Count;

            // Get recent lessons
            var recentLessons = teacherLessons
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .Select(l => new RecentLessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    //GradeLevel = l.GradeLevel?.Name ?? "N/A",
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                }).ToList();

            return new TeacherDashboardDto
            {
                TotalQuizzesCreated = totalQuizzesCreated,
                TotalQuizzesTaken = totalQuizzesTaken,
                AverageQuizScore = averageQuizScore,
                RecentQuizzes = recentQuizzes,
                TotalLessonsCreated = totalLessonsCreated,
                RecentLessons = recentLessons,
                TotalStudentsEngaged = uniqueStudents,
                QuizPerformance = quizPerformance
            };
        }

        public async Task<IEnumerable<Quizs>> GetQuizzesByTeacherAsync(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentException("Teacher ID must be greater than 0", nameof(teacherId));

            return await _quizRepository.GetQuizzesByTeacherAsync(teacherId);
        }
    }
}