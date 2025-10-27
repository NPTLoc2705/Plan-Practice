using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos
{
    public class TeacherDashboardDto
    {
        // Quiz Statistics
        public int TotalQuizzesCreated { get; set; }
        public int TotalQuizzesTaken { get; set; }
        public double AverageQuizScore { get; set; }
        public List<QuizStatDto> RecentQuizzes { get; set; } = new List<QuizStatDto>();
        
        // Lesson Statistics
        public int TotalLessonsCreated { get; set; }
        public List<RecentLessonDto> RecentLessons { get; set; } = new List<RecentLessonDto>();
        
        // Overall Statistics
        public int TotalStudentsEngaged { get; set; }
        public QuizPerformanceBreakdown QuizPerformance { get; set; } = new QuizPerformanceBreakdown();
    }

    public class QuizStatDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TimesTaken { get; set; }
        public double AverageScore { get; set; }
    }

    public class RecentLessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string GradeLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class QuizPerformanceBreakdown
    {
        public int ExcellentCount { get; set; }  // 90-100%
        public int GoodCount { get; set; }       // 70-89%
        public int AverageCount { get; set; }    // 50-69%
        public int BelowAverageCount { get; set; } // Below 50%
    }
}
