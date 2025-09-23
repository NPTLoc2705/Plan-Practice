using BusinessObject.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizManagementController : ControllerBase
    {
        private readonly IQuizManagementService _quizManagementService;

        public QuizManagementController(IQuizManagementService quizManagementService)
        {
            _quizManagementService = quizManagementService;
        }

        [HttpGet("quiz/{quizId}/details")]
        public async Task<ActionResult<QuizDto>> GetQuizWithDetails(int quizId)
        {
            try
            {
                var quizWithDetails = await _quizManagementService.GetQuizWithQuestionsAndAnswersAsync(quizId);
                var quiz = quizWithDetails.Quiz;
                var quizDto = new QuizDto
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    Description = quiz.Description,
                    CreatedAt = quiz.CreatedAt,
                    Questions = quizWithDetails.Questions.Select(q => new QuestionDto
                    {
                        Id = q.Question.Id, // Fixed: Accessing the 'Id' property of the 'Question' object inside 'QuestionWithAnswers'
                        Content = q.Question.Content,
                        QuizId = q.Question.QuizId,
                        Answers = q.Answers.Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList()
                };
                return Ok(quizDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving quiz details", error = ex.Message });
            }
        }

        [HttpPost("quiz/create-complete")]
        public async Task<ActionResult<QuizDto>> CreateCompleteQuiz([FromBody] CreateQuizRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quizWithDetails = await _quizManagementService.CreateCompleteQuizAsync(request);
                var quiz = quizWithDetails.Quiz;
                var quizDto = new QuizDto
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    Description = quiz.Description,
                    CreatedAt = quiz.CreatedAt,
                    Questions = quizWithDetails.Questions.Select(q => new QuestionDto
                    {
                        Id = q.Question.Id, // Fixed: Accessing the 'Id' property of the 'Question' object inside 'QuestionWithAnswers'
                        Content = q.Question.Content,
                        QuizId = q.Question.QuizId,
                        Answers = q.Answers.Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList()
                };
                return CreatedAtAction(nameof(GetQuizWithDetails), new { quizId = quizDto.Id }, quizDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the quiz", error = ex.Message });
            }
        }

        [HttpPost("quiz/submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuizAnswers([FromBody] SubmitQuizRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _quizManagementService.SubmitQuizAnswersAsync(request);
                var resultDto = new QuizResultDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    QuizId = result.QuizId,
                    Score = result.Score,
                    CompletedAt = result.CompletedAt,
                    UserAnswers = result.UserAnswers.Select(ua => new UserAnswerDto
                    {
                        Id = ua.Id,
                        QuizResultId = ua.QuizResultId,
                        QuestionId = ua.QuestionId,
                        AnswerId = ua.AnswerId
                    }).ToList()
                };
                return Ok(resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting quiz answers", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}/history")]
        public async Task<ActionResult<IEnumerable<QuizResultDto>>> GetUserQuizHistory(int userId)
        {
            try
            {
                var history = await _quizManagementService.GetUserQuizHistoryAsync(userId);
                var historyDtos = history.Select(result => new QuizResultDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    QuizId = result.QuizId,
                    Score = result.Score,
                    CompletedAt = result.CompletedAt,
                    UserAnswers = result.UserAnswers.Select(ua => new UserAnswerDto
                    {
                        Id = ua.Id,
                        QuizResultId = ua.QuizResultId,
                        QuestionId = ua.QuestionId,
                        AnswerId = ua.AnswerId
                    }).ToList()
                });
                return Ok(historyDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user quiz history", error = ex.Message });
            }
        }

        [HttpGet("quiz/{quizId}/statistics")]
        public async Task<ActionResult<QuizStatistics>> GetQuizStatistics(int quizId)
        {
            try
            {
                var statistics = await _quizManagementService.GetQuizStatisticsAsync(quizId);
                return Ok(statistics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving quiz statistics", error = ex.Message });
            }
        }
    }
}