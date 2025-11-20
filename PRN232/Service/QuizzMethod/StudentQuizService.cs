using BusinessObject.Dtos.Quiz;
using BusinessObject.Quiz;
using Repository.Interface;
using Repository.Interface.Student;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class StudentQuizService : IStudentQuizService
    {

        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IUserAnswerRepository _userAnswerRepository;

        //student
        private readonly IQuizStudentRepository _quizStudentRepository;
        private readonly IQuestionStudentRepository _questionStudentRepository;
        private readonly IAnswerStudentRepository _answerStudentRepository;
        private readonly IQuizResultStudentRepository _quizResultStudentRepository;
        private readonly IUserAnswerStudentRepository _userAnswerStudentRepository;
        private readonly IQuizStatisticsStudentRepository _statisticsStudentRepository;

        public StudentQuizService(
            IQuizRepository quizRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository,
            IQuizResultRepository quizResultRepository,
            IUserAnswerRepository userAnswerRepository,

            //Student
            IQuizStudentRepository quizStudentRepository,
            IQuestionStudentRepository questionStudentRepository,
            IAnswerStudentRepository answerStudentRepository,
            IQuizResultStudentRepository quizResultStudentRepository,
            IUserAnswerStudentRepository userAnswerStudentRepository,
            IQuizStatisticsStudentRepository statisticsStudentRepository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _quizResultRepository = quizResultRepository;
            _userAnswerRepository = userAnswerRepository;

            _quizStudentRepository = quizStudentRepository;
            _questionStudentRepository = questionStudentRepository;
            _answerStudentRepository = answerStudentRepository;
            _quizResultStudentRepository = quizResultStudentRepository;
            _userAnswerStudentRepository = userAnswerStudentRepository;
            _statisticsStudentRepository = statisticsStudentRepository;
        }

        public async Task<IEnumerable<QuizInfoDto>> GetAvailableQuizzesAsync(int userId)
        {
            var result = new List<QuizInfoDto>();

            try
            {
                if (_quizRepository == null)
                    throw new NullReferenceException("_quizRepository is null");
                if (_questionStudentRepository == null)
                    throw new NullReferenceException("_questionStudentRepository is null");
                if (_quizResultStudentRepository == null)
                    throw new NullReferenceException("_quizResultStudentRepository is null");

                var quizzes = await _quizRepository.GetTotalQuizzesAsync()
                              ?? throw new NullReferenceException("GetTotalQuizzesAsync() returned null");

                foreach (var quiz in quizzes)
                {
                    if (quiz == null)
                        continue;

                    var questionCount = await _questionStudentRepository.GetQuestionCountByQuizIdAsync(quiz.Id);
                    var lastResult = await _quizResultStudentRepository.GetLatestQuizResultByUserAndQuizAsync(userId, quiz.Id);

                    result.Add(new QuizInfoDto
                    {
                        Id = quiz.Id,
                        Title = quiz.Title,
                        Description = quiz.Description,
                        TotalQuestions = questionCount,
                        CreatedAt = quiz.CreatedAt,
                        HasAttempted = lastResult != null,
                        LastResult = lastResult != null ? new QuizResultDto
                        {
                            Id = lastResult.Id,
                            Score = lastResult.Score,
                            TotalQuestions = questionCount,
                            Percentage = CalculatePercentage(lastResult.Score, questionCount),
                            CompletedAt = lastResult.CompletedAt
                        } : null
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableQuizzesAsync: {ex}");
                throw; 
            }

            return result;
        }

        public async Task<QuizDetailDto> GetQuizForTakingAsync(int quizId, int userId)
        {
            var quiz = await _quizStudentRepository.GetQuizWithQuestionsAndAnswersAsync(quizId);
            if (quiz == null)
                return null;

            var inProgressResult = await _quizResultStudentRepository.GetInProgressQuizResultAsync(userId, quizId);

            var questionDtos = new List<QuestionDto>();
            foreach (var question in quiz.Questions)
            {
                var answers = await _answerStudentRepository.GetAnswersByQuestionIdAsync(question.Id);
                questionDtos.Add(new QuestionDto
                {
                    Id = question.Id,
                    Content = question.Content,
                    Answers = answers.Select(a => new AnswerOptionDto
                    {
                        Id = a.Id,
                        Content = a.Content
                    }).ToList()
                });
            }

            return new QuizDetailDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = questionDtos,
                CurrentQuizResultId = inProgressResult?.Id
            };
        }
        public async Task<QuizResultDto> SubmitQuizAsync(int userId, SubmitQuizDto dto)
        {
            var quiz = await _quizStudentRepository.GetQuizWithQuestionsAndAnswersAsync(dto.QuizId);
            if (quiz == null)
                throw new Exception("Quiz does not exist");

            var quizResult = new QuizResult
            {
                UserId = userId,
                QuizId = dto.QuizId,
                Score = 0,
                CompletedAt = DateTime.UtcNow
            };

            await _quizResultRepository.CreateQuizResultAsync(quizResult);

            int correctCount = 0;
            var details = new List<QuestionResultDto>();
            var userAnswers = new List<UserAnswer>();

            foreach (var question in quiz.Questions)
            {
                var userAnswerDto = dto.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var correctAnswer = await _answerStudentRepository.GetCorrectAnswerByQuestionIdAsync(question.Id);

                if (userAnswerDto != null)
                {
                    var userAnswer = new UserAnswer
                    {
                        QuizResultId = quizResult.Id,
                        QuestionId = question.Id,
                        AnswerId = userAnswerDto.AnswerId
                    };
                    userAnswers.Add(userAnswer);

                    bool isCorrect = await _answerStudentRepository.IsAnswerCorrectAsync(userAnswerDto.AnswerId);
                    if (isCorrect)
                        correctCount++;

                    var selectedAnswer = await _answerRepository.GetAnswerByIdAsync(userAnswerDto.AnswerId);
                    details.Add(new QuestionResultDto
                    {
                        QuestionId = question.Id,
                        QuestionContent = question.Content,
                        UserAnswerId = userAnswerDto.AnswerId,
                        UserAnswerContent = selectedAnswer?.Content,
                        CorrectAnswerId = correctAnswer?.Id ?? 0,
                        CorrectAnswerContent = correctAnswer?.Content,
                        IsCorrect = isCorrect
                    });
                }
                else
                {
                    details.Add(new QuestionResultDto
                    {
                        QuestionId = question.Id,
                        QuestionContent = question.Content,
                        UserAnswerId = null,
                        UserAnswerContent = "not answer",
                        CorrectAnswerId = correctAnswer?.Id ?? 0,
                        CorrectAnswerContent = correctAnswer?.Content,
                        IsCorrect = false
                    });
                }
            }

            await _userAnswerStudentRepository.CreateUserAnswersAsync(userAnswers);

            quizResult.Score = correctCount;
            await _quizResultRepository.UpdateQuizResultAsync(quizResult);

            return new QuizResultDto
            {
                Id = quizResult.Id,
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                Score = correctCount,
                TotalQuestions = quiz.Questions.Count,
                CorrectAnswers = correctCount,
                Percentage = CalculatePercentage(correctCount, quiz.Questions.Count),
                CompletedAt = quizResult.CompletedAt,
                Details = details
            };
        }
        public async Task<QuizResultDto> GetQuizResultAsync(int resultId, int userId)
        {
            var result = await _quizResultStudentRepository.GetQuizResultWithDetailsAsync(resultId);
            if (result == null || result.UserId != userId)
                return null;

            var details = new List<QuestionResultDto>();

            foreach (var question in result.Quiz.Questions)
            {
                var userAnswer = result.UserAnswers.FirstOrDefault(ua => ua.QuestionId == question.Id);
                var correctAnswer = await _answerStudentRepository.GetCorrectAnswerByQuestionIdAsync(question.Id);

                details.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    QuestionContent = question.Content,
                    UserAnswerId = userAnswer?.AnswerId,
                    UserAnswerContent = userAnswer?.Answer?.Content ?? "not answer",
                    CorrectAnswerId = correctAnswer?.Id ?? 0,
                    CorrectAnswerContent = correctAnswer?.Content,
                    IsCorrect = userAnswer?.Answer?.IsCorrect ?? false
                });
            }

            return new QuizResultDto
            {
                Id = result.Id,
                QuizId = result.QuizId,
                QuizTitle = result.Quiz.Title,
                Score = result.Score,
                TotalQuestions = result.Quiz.Questions.Count,
                CorrectAnswers = result.Score,
                Percentage = CalculatePercentage(result.Score, result.Quiz.Questions.Count),
                CompletedAt = result.CompletedAt,
                Details = details
            };
        }

        // quiz history
        public async Task<IEnumerable<QuizHistoryDto>> GetUserQuizHistoryAsync(int userId)
        {
            var results = await _quizResultStudentRepository.GetCompletedQuizResultsByUserAsync(userId);
            var history = new List<QuizHistoryDto>();

            foreach (var result in results)
            {
                var questionCount = await _questionStudentRepository.GetQuestionCountByQuizIdAsync(result.QuizId);
                history.Add(new QuizHistoryDto
                {
                    ResultId = result.Id,
                    QuizId = result.QuizId,
                    QuizTitle = result.Quiz.Title,
                    Score = result.Score,
                    TotalQuestions = questionCount,
                    Percentage = CalculatePercentage(result.Score, questionCount),
                    CompletedAt = result.CompletedAt
                });
            }

            return history;
        }

        // quiz statistic
        public async Task<QuizStatisticsDto> GetQuizStatisticsAsync(int quizId, int userId)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
            if (quiz == null)
                return null;

            var userBestScore = await _statisticsStudentRepository.GetUserBestScoreAsync(userId, quizId);
            var userAttempts = await _statisticsStudentRepository.GetUserAttemptCountAsync(userId, quizId);

            var allResults = await _quizResultStudentRepository.GetQuizResultsByQuizIdAsync(quizId);
            var completedResults = allResults.Where(r => r.Score > 0).ToList();

            return new QuizStatisticsDto
            {
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                TotalAttempts = completedResults.Count,
                AverageScore = completedResults.Any() ? (decimal)completedResults.Average(r => r.Score) : 0,
                HighestScore = completedResults.Any() ? completedResults.Max(r => r.Score) : 0,
                YourBestScore = userBestScore,
                YourAttempts = userAttempts
            };
        }

        public async Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId)
        {
            return await _quizResultStudentRepository.HasUserAttemptedQuizAsync(userId, quizId);
        }

        private decimal CalculatePercentage(int score, int total)
        {
            if (total == 0) return 0;
            return Math.Round((decimal)score / total * 100, 2);
        }
    }



}
