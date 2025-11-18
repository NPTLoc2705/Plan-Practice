using BusinessObject.Dtos.Quiz;
using BusinessObject.Dtos.LessonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IGeminiService
    {
        Task<QuizGenerationResult> GenerateQuizFromLessonAsync(string lessonContent, int numberOfQuestions = 5);
        
        Task<GeneratedLessonPlannerResult> GenerateLessonPlannerAsync(GenerateLessonPlannerRequest request);
    }
}
