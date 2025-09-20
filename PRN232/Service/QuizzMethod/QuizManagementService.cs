using BusinessObject.Quiz;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class QuizManagementService : IQuizManagementService
    {
        private readonly IQuizService _quizService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IQuizResultService _quizResultService;
        private readonly IUserAnswerService _userAnswerService;

        public QuizManagementService(
            IQuizService quizService,
            IQuestionService questionService,
            IAnswerService answerService,
            IQuizResultService quizResultService,
            IUserAnswerService userAnswerService)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _answerService = answerService ?? throw new ArgumentNullException(nameof(answerService));
            _quizResultService = quizResultService ?? throw new ArgumentNullException(nameof(quizResultService));
            _userAnswerService = userAnswerService ?? throw new ArgumentNullException(nameof(userAnswerService));
        }

        public async Task<QuizWithDetails> GetQuizWithQuestionsAndAnswersAsync(int quizId)
        {
            var quiz = await _quizService.GetQuizByIdAsync(quizId);
            if (quiz == null)
                throw new InvalidOperationException($"Quiz with ID {quizId} not found");

            var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            var quizWithDetails = new QuizWithDetails
            {
                Quiz = quiz,
                Questions = new List<QuestionWithAnswers>()
            };

            foreach (var question in questions)
            {
                var answers = await _answerService.GetAnswersByQuestionIdAsync(question.Id);
                quizWithDetails.Questions.Add(new QuestionWithAnswers
                {
                    Question = question,
                    Answers = answers.ToList()
                });
            }

            return quizWithDetails;
        }

        public async Task<QuizWithDetails> CreateCompleteQuizAsync(CreateQuizRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Quiz title is required", nameof(request));

            if (request.Questions == null || !request.Questions.Any())
                throw new ArgumentException("At least one question is required", nameof(request));

            // Create the quiz
            var quiz = new Quiz
            {
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            var createdQuiz = await _quizService.CreateQuizAsync(quiz);

            // Create questions and answers
            var questionsWithAnswers = new List<QuestionWithAnswers>();

            foreach (var questionRequest in request.Questions)
            {
                if (string.IsNullOrWhiteSpace(questionRequest.Content))
                    throw new ArgumentException("Question content is required");

                if (questionRequest.Answers == null || !questionRequest.Answers.Any())
                    throw new ArgumentException("At least one answer is required for each question");

                // Create the question
                var question = new Question
                {
                    QuizId = createdQuiz.Id,
                    Content = questionRequest.Content
                };

                var createdQuestion = await _questionService.CreateQuestionAsync(question);

                // Create answers
                var answers = new List<Answer>();
                foreach (var answerRequest in questionRequest.Answers)
                {
                    if (string.IsNullOrWhiteSpace(answerRequest.Content))
                        throw new ArgumentException("Answer content is required");

                    var answer = new Answer
                    {
                        QuestionId = createdQuestion.Id,
                        Content = answerRequest.Content,
                        IsCorrect = answerRequest.IsCorrect
                    };

                    var createdAnswer = await _answerService.CreateAnswerAsync(answer);
                    answers.Add(createdAnswer);
                }

                questionsWithAnswers.Add(new QuestionWithAnswers
                {
                    Question = createdQuestion,
                    Answers = answers
                });
            }

            return new QuizWithDetails
            {
                Quiz = createdQuiz,
                Questions = questionsWithAnswers
            };
        }

        public async Task<QuizResult> SubmitQuizAnswersAsync(SubmitQuizRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.UserId <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(request));

            if (request.QuizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(request));

            if (request.Answers == null || !request.Answers.Any())
                throw new ArgumentException("At least one answer is required", nameof(request));

            // Get the quiz to verify it exists
            var quiz = await _quizService.GetQuizByIdAsync(request.QuizId);
            if (quiz == null)
                throw new InvalidOperationException($"Quiz with ID {request.QuizId} not found");

            // Get all questions for this quiz
            var questions = await _questionService.GetQuestionsByQuizIdAsync(request.QuizId);
            var totalQuestions = questions.Count();

            if (totalQuestions == 0)
                throw new InvalidOperationException("Quiz has no questions");

            // Calculate score
            int correctAnswers = 0;
            var userAnswers = new List<UserAnswer>();

            foreach (var submission in request.Answers)
            {
                // Verify the question belongs to this quiz
                var question = questions.FirstOrDefault(q => q.Id == submission.QuestionId);
                if (question == null)
                    throw new ArgumentException($"Question with ID {submission.QuestionId} does not belong to quiz {request.QuizId}");

                // Get the answer to check if it's correct
                var answer = await _answerService.GetAnswerByIdAsync(submission.AnswerId);
                if (answer == null)
                    throw new ArgumentException($"Answer with ID {submission.AnswerId} not found");

                if (answer.QuestionId != submission.QuestionId)
                    throw new ArgumentException($"Answer with ID {submission.AnswerId} does not belong to question {submission.QuestionId}");

                // Check if answer is correct
                if (answer.IsCorrect)
                    correctAnswers++;
            }

            // Calculate score percentage
            var score = (int)Math.Round((double)correctAnswers / totalQuestions * 100);

            // Create quiz result first
            var quizResult = new QuizResult
            {
                UserId = request.UserId,
                QuizId = request.QuizId,
                Score = score,
                CompletedAt = DateTime.UtcNow
            };

            var createdResult = await _quizResultService.CreateQuizResultAsync(quizResult);

            // Now create user answers with the QuizResultId
            foreach (var submission in request.Answers)
            {
                var userAnswer = new UserAnswer
                {
                    QuizResultId = createdResult.Id,
                    QuestionId = submission.QuestionId,
                    AnswerId = submission.AnswerId
                };

                userAnswers.Add(userAnswer);
            }

            // Save all user answers
            foreach (var userAnswer in userAnswers)
            {
                await _userAnswerService.CreateUserAnswerAsync(userAnswer);
            }

            return createdResult;
        }

        public async Task<IEnumerable<QuizResult>> GetUserQuizHistoryAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(userId));

            return await _quizResultService.GetQuizResultsByUserIdAsync(userId);
        }

        public async Task<QuizStatistics> GetQuizStatisticsAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));

            var quiz = await _quizService.GetQuizByIdAsync(quizId);
            if (quiz == null)
                throw new InvalidOperationException($"Quiz with ID {quizId} not found");

            var quizResults = await _quizResultService.GetQuizResultsByQuizIdAsync(quizId);
            var resultsList = quizResults.ToList();

            if (!resultsList.Any())
            {
                return new QuizStatistics
                {
                    TotalAttempts = 0,
                    AverageScore = 0,
                    HighestScore = 0,
                    LowestScore = 0,
                    QuestionStats = new List<QuestionStatistics>()
                };
            }

            var statistics = new QuizStatistics
            {
                TotalAttempts = resultsList.Count,
                AverageScore = resultsList.Average(r => r.Score),
                HighestScore = resultsList.Max(r => r.Score),
                LowestScore = resultsList.Min(r => r.Score),
                QuestionStats = new List<QuestionStatistics>()
            };

            // Get question statistics
            var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            foreach (var question in questions)
            {
                var answers = await _answerService.GetAnswersByQuestionIdAsync(question.Id);
                var correctAnswer = answers.FirstOrDefault(a => a.IsCorrect);

                if (correctAnswer != null)
                {
                    // Get all quiz results for this quiz
                    var allQuizResults = resultsList.Select(r => r.Id).ToList();
                    var questionUserAnswers = new List<UserAnswer>();

                    // Get user answers for each quiz result
                    foreach (var quizResultId in allQuizResults)
                    {
                        var userAnswers = await _userAnswerService.GetUserAnswersByQuizResultIdAsync(quizResultId);
                        questionUserAnswers.AddRange(userAnswers.Where(ua => ua.QuestionId == question.Id));
                    }

                    var correctUserAnswers = questionUserAnswers.Count(ua => ua.AnswerId == correctAnswer.Id);

                    statistics.QuestionStats.Add(new QuestionStatistics
                    {
                        QuestionId = question.Id,
                        Content = question.Content,
                        TotalAnswers = questionUserAnswers.Count,
                        CorrectAnswers = correctUserAnswers,
                        CorrectPercentage = questionUserAnswers.Count > 0 ? 
                            (double)correctUserAnswers / questionUserAnswers.Count * 100 : 0
                    });
                }
            }

            return statistics;
        }
    }
}