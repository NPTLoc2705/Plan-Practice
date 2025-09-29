using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos
{
    public class TeacherDashboardDto
    {
        public int TotalTestsCreated { get; set; }
        public int TotalTestsTaken { get; set; }
        public double AverageScore { get; set; }
        public List<QuizStatDto> RecentQuizzes { get; set; } = new List<QuizStatDto>();
    }

    public class QuizStatDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TimesTaken { get; set; }
        public double AverageScore { get; set; }
    }
}
