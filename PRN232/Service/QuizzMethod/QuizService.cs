using BusinessObject.Dtos;
using BusinessObject.Dtos.Quiz;
using BusinessObject.Lesson;
using BusinessObject.Quiz;
using Repository.Interface;
using Service.Interface;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILessonPlannerService _lessonPlannerService;
        private readonly IGeminiService _geminiService;
        private readonly ILessonPlannerRepository _lessonPlannerRepository; 


        public QuizService(IQuizRepository quizRepository, ILessonPlannerService lessonPlannerService, IGeminiService geminiService, ILessonPlannerRepository lessonPlannerRepository)
        {
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
            _lessonPlannerService = lessonPlannerService ?? throw new ArgumentNullException(nameof(lessonPlannerService));
            _geminiService = geminiService ?? throw new ArgumentNullException(nameof(geminiService));
            _lessonPlannerRepository = lessonPlannerRepository ?? throw new ArgumentNullException(nameof(lessonPlannerRepository)); 

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

        //===============================AI===============================//

        public async Task<Quizs> CreateQuizWithAIAsync(
            int lessonPlannerId,
            string title,
            string description,
            int numberOfQuestions)
        {
            // ========== PHASE 1: PREPARATION & VALIDATION ==========

            // Validate input parameters
            ValidateAIQuizInput(lessonPlannerId, title, numberOfQuestions);

            // 1. Get LessonPlanner from repository
            var lessonPlanner = await _lessonPlannerRepository.GetLessonPlannerByIdAsync(lessonPlannerId);

            if (lessonPlanner == null)
                throw new ArgumentException($"Lesson Planner with ID {lessonPlannerId} not found");

            // 2. Extract lesson content
            var lessonContent = ExtractLessonContent(lessonPlanner);

            if (string.IsNullOrWhiteSpace(lessonContent))
                throw new ArgumentException("Lesson content is empty or too short. Cannot generate quiz.");

            // Additional check: content should be long enough to generate meaningful questions
            if (lessonContent.Length < 100)
                throw new ArgumentException(
                    $"Lesson content is too short ({lessonContent.Length} characters). Minimum 100 characters required to generate meaningful quiz questions."
                );

            // 3. Generate quiz using Gemini AI
            // (AI validation happens inside GeminiService)
            QuizGenerationResult aiResult;
            try
            {
                aiResult = await _geminiService.GenerateQuizFromLessonAsync(
                    lessonContent,
                    numberOfQuestions
                );
            }
            catch (InvalidOperationException ex)
            {
                // Re-throw validation errors from AI with more context
                throw new InvalidOperationException(
                    $"AI quiz generation validation failed: {ex.Message}",
                    ex
                );
            }
            catch (Exception ex)
            {
                // Handle other AI errors
                throw new Exception(
                    $"Failed to generate quiz using AI: {ex.Message}",
                    ex
                );
            }

            // ========== PHASE 2: BUILD QUIZ ENTITY ==========

            // 4. Create Quiz entity
            var quiz = new Quizs
            {
                Title = title?.Trim() ?? throw new ArgumentNullException(nameof(title)),
                Description = description?.Trim(),
                LessonPlannerId = lessonPlannerId,
                CreatedAt = DateTime.UtcNow,
                Questions = new List<Question>()
            };

            // 5. Convert AI results to entities
            foreach (var aiQuestion in aiResult.Questions)
            {
                var question = new Question
                {
                    Content = aiQuestion.Content.Trim(),
                    Answers = new List<Answer>()
                };

                foreach (var aiAnswer in aiQuestion.Answers)
                {
                    question.Answers.Add(new Answer
                    {
                        Content = aiAnswer.Content.Trim(),
                        IsCorrect = aiAnswer.IsCorrect
                    });
                }

                quiz.Questions.Add(question);
            }

            // Final sanity check before saving
            ValidateQuizEntityBeforeSave(quiz);

            // ========== PHASE 3: SAVE TO DATABASE WITH TRANSACTION ==========

            try
            {
                // Repository method should handle transaction internally
                // If your repository doesn't have transaction support, wrap it here
                await _quizRepository.CreateQuizAsync(quiz);

                return quiz;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to save AI-generated quiz to database: {ex.Message}. " +
                    $"Quiz title: '{title}', Questions: {quiz.Questions.Count}",
                    ex
                );
            }
        }

        // ========== VALIDATION METHODS ==========

        private void ValidateAIQuizInput(int lessonPlannerId, string title, int numberOfQuestions)
        {
            if (lessonPlannerId <= 0)
                throw new ArgumentException("LessonPlannerId must be greater than 0", nameof(lessonPlannerId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Quiz title cannot be empty", nameof(title));

            if (title.Trim().Length < 5)
                throw new ArgumentException("Quiz title must be at least 5 characters long", nameof(title));

            if (title.Trim().Length > 200)
                throw new ArgumentException("Quiz title cannot exceed 200 characters", nameof(title));

            if (numberOfQuestions < 1 || numberOfQuestions > 20)
                throw new ArgumentException(
                    $"Number of questions must be between 1 and 20, but got {numberOfQuestions}",
                    nameof(numberOfQuestions)
                );
        }

        private void ValidateQuizEntityBeforeSave(Quizs quiz)
        {
            // This is a final sanity check before database save
            // Should rarely fail if AI validation worked properly

            if (quiz.Questions == null || !quiz.Questions.Any())
                throw new InvalidOperationException("Quiz has no questions");

            foreach (var question in quiz.Questions)
            {
                if (question.Answers == null || question.Answers.Count != 4)
                    throw new InvalidOperationException(
                        $"Question '{question.Content}' must have exactly 4 answers"
                    );

                var correctCount = question.Answers.Count(a => a.IsCorrect);
                if (correctCount != 1)
                    throw new InvalidOperationException(
                        $"Question '{question.Content}' must have exactly 1 correct answer, but has {correctCount}"
                    );
            }
        }

        // ========== CONTENT EXTRACTION (EXISTING - WITH EXTENSION METHODS) ==========

        private string ExtractLessonContent(LessonPlanner lessonPlanner)
        {
            var content = new StringBuilder();

            content.AppendLine($"Title: {lessonPlanner.Title}");

            if (!string.IsNullOrEmpty(lessonPlanner.UnitName))
                content.AppendLine($"Unit: {lessonPlanner.UnitName}");

            if (!string.IsNullOrEmpty(lessonPlanner.Description))
                content.AppendLine($"Description: {lessonPlanner.Description}");

            if (!string.IsNullOrEmpty(lessonPlanner.Content))
            {
                var plainContent = StripHtml(lessonPlanner.Content);
                content.AppendLine($"\nContent:\n{plainContent}");
            }

            // Add objectives - using extension methods or direct access
            if (lessonPlanner.Objectives?.Any() == true)
            {
                content.AppendLine("\nObjectives:");
                foreach (var obj in lessonPlanner.Objectives)
                {
                    // Use CustomContent first, then template, then snapshot
                    var objectiveContent = obj.CustomContent
                        ?? obj.GetObjectiveContent() // Extension method
                        ?? obj.SnapshotContent;

                    if (!string.IsNullOrEmpty(objectiveContent))
                    {
                        content.AppendLine($"- {objectiveContent}");
                    }
                }
            }

            // Add skills - using extension methods
            if (lessonPlanner.Skills?.Any() == true)
            {
                content.AppendLine("\nSkill:");
                foreach (var skill in lessonPlanner.Skills)
                {
                    var skillContent = skill.CustomContent
                        ?? skill.GetSkillDescription() // Extension method
                        ?? skill.SnapshotDescription;

                    if (!string.IsNullOrEmpty(skillContent))
                    {
                        content.AppendLine($"- {skillContent}");
                    }
                }
            }

            // Add attitudes (optional)
            if (lessonPlanner.Attitudes?.Any() == true)
            {
                content.AppendLine("\nAttitudes:");
                foreach (var attitude in lessonPlanner.Attitudes)
                {
                    var attitudeContent = attitude.CustomContent
                        ?? attitude.GetAttitudeContent() // Extension method
                        ?? attitude.SnapshotContent;

                    if (!string.IsNullOrEmpty(attitudeContent))
                    {
                        content.AppendLine($"- {attitudeContent}");
                    }
                }
            }

            // Add activity stages (optional)
            if (lessonPlanner.ActivityStages?.Any() == true)
            {
                content.AppendLine("\nActivity Stages:");
                foreach (var stage in lessonPlanner.ActivityStages.OrderBy(s => s.DisplayOrder))
                {
                    if (!string.IsNullOrEmpty(stage.StageName))
                    {
                        content.AppendLine($"\n{stage.StageName}:");

                        if (stage.ActivityItems?.Any() == true)
                        {
                            foreach (var item in stage.ActivityItems.OrderBy(i => i.DisplayOrder))
                            {
                                var itemContent = StripHtml(item.Content ?? "");
                                if (!string.IsNullOrEmpty(itemContent))
                                {
                                    content.AppendLine($"  - {itemContent}");
                                }
                            }
                        }
                    }
                }
            }

            return content.ToString();
        }

        private string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // Remove HTML tags
            var text = Regex.Replace(html, "<.*?>", string.Empty);

            // Decode HTML entities
            text = System.Net.WebUtility.HtmlDecode(text);

            // Remove extra whitespace
            text = Regex.Replace(text, @"\s+", " ");

            return text.Trim();
        }
    }
}