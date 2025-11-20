using BusinessObject;
using BusinessObject.Dtos;
using BusinessObject.Dtos.Quiz;
using BusinessObject.Lesson;
using BusinessObject.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Service.Interface;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ICoinService _coinService;
        private const int AIGenerationCoinCost = 50;

        public QuizController(IQuizService quizService, ICoinService coin)
        {
            _quizService = quizService;
            _coinService = coin;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAllQuizzes()
        {
            try
            {
                
                var quizzes = await _quizService.GetAllQuizzesAsync();
                var quizDtos = quizzes.Select(q => new QuizDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    CreatedAt = q.CreatedAt,
                    LessonPlannerId = q.LessonPlannerId
                });
                return Ok(quizDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving quizzes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz == null)
                    return NotFound(new { message = $"Quiz with ID {id} not found" });

                var quizDto = new QuizDto
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    Description = quiz.Description,
                    CreatedAt = quiz.CreatedAt,
                    LessonPlannerId = quiz.LessonPlannerId
                };
                return Ok(quizDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the quiz", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] QuizDto quizDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quiz = new Quizs
                {
                    Title = quizDto.Title,
                    Description = quizDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    LessonPlannerId = quizDto.LessonPlannerId
                };

                await _quizService.CreateQuizAsync(quiz);

                return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<QuizDto>> UpdateQuiz(QuizDto quizDto)
        {
            try
            {
                if (quizDto.Id != quizDto.Id)
                    return BadRequest(new { message = "Quiz ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var quiz = new Quizs
                {
                    Id = quizDto.Id,
                    Title = quizDto.Title,
                    Description = quizDto.Description,
                    LessonPlannerId = quizDto.LessonPlannerId,
                    CreatedAt=DateTime.UtcNow,
                };

                await _quizService.UpdateQuizAsync(quiz);
                return Ok(quiz);
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
                return StatusCode(500, new { message = "An error occurred while updating the quiz", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]

        public async Task<ActionResult> DeleteQuiz(int id)
        {
            try
            {
                await _quizService.DeleteQuizAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the quiz", error = ex.Message });
            }
        }

        [HttpGet("teacher/me/dashboard")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<BusinessObject.Dtos.TeacherDashboardDto>> GetTeacherDashboard()
        {
            try
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var teacherId))
                {
                    return Unauthorized(new { message = "Invalid user authentication" });
                }

                var dashboard = await _quizService.GetTeacherDashboardStatsAsync(teacherId);
                return Ok(new { success = true, data = dashboard, message = "Dashboard data retrieved successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving teacher dashboard", error = ex.Message });
            }
        }

        [HttpGet("teacher/me")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesByTeacher()
        {
            try
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var teacherId))
                {
                    return Unauthorized(new { message = "Invalid user authentication" });
                }

                var quizzes = await _quizService.GetQuizzesByTeacherAsync(teacherId);
                var quizDtos = quizzes.Select(q => new QuizDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    LessonPlannerId = q.LessonPlannerId,
                    TotalQuestion = q.Questions?.Count ?? 0
                });
                return Ok(new
                {
                    success = true,
                    data = quizDtos,
                    message = "Quizzes retrieved successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving teacher quizzes", error = ex.Message });
            }
        }


        //============================================AI============================================//

        [Authorize(Roles = "Teacher")]
        [HttpPost("generate-with-ai")]
        public async Task<IActionResult> CreateQuizWithAI([FromBody] GenerateQuizDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input data",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            try
            {
                var currentBalance = await _coinService.GetUserCoinBalance(userId);
                if (currentBalance < AIGenerationCoinCost)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Insufficient coins. You need {AIGenerationCoinCost} coins to generate AI quiz.",
                        coinBalance = currentBalance
                    });
                }

                var deductionSucceeded = await _coinService.DeductCoinsForLessonGeneration(userId);
                if (!deductionSucceeded)
                {
                    var refreshedBalance = await _coinService.GetUserCoinBalance(userId);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Unable to deduct coins. Please refresh your balance and try again.",
                        coinBalance = refreshedBalance
                    });
                }

             //   QuizResponse quiz = null;

                try
                {
                   var  quiz = await _quizService.CreateQuizWithAIAsync(
                        dto.LessonPlannerId,
                        dto.Title,
                        dto.Description,
                        dto.NumberOfQuestions
                    );

                    var newBalance = await _coinService.GetUserCoinBalance(userId);

                    return CreatedAtAction(
                        nameof(GetQuiz),
                        new { id = quiz.Id },
                        new
                        {
                            success = true,
                            message = "Quiz generated successfully by AI.",
                            newBalance,
                            data = new
                            {
                                quiz.Id,
                                quiz.Title,
                                quiz.Description,
                                quiz.CreatedAt,
                                quiz.LessonPlannerId,
                                questionsCount = quiz.Questions.Count,
                                questions = quiz.Questions.Select(q => new
                                {
                                    q.Id,
                                    q.Content,
                                    answersCount = q.Answers.Count,
                                    answers = q.Answers.Select(a => new
                                    {
                                        a.Id,
                                        a.Content,
                                        a.IsCorrect
                                    })
                                })
                            }
                        }
                    );
                }
                catch (InvalidOperationException ex)
                {
                    await _coinService.RefundCoins(userId, AIGenerationCoinCost);
                    return BadRequest(new
                    {
                        success = false,
                        message = "AI validation failed.",
                        error = ex.Message,
                        hint = "The AI could not generate a valid quiz. Please check your lesson content or try again."
                    });
                }
                catch (ArgumentException ex)
                {
                    await _coinService.RefundCoins(userId, AIGenerationCoinCost);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid input.",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    await _coinService.RefundCoins(userId, AIGenerationCoinCost);
                    if (ex.Message.Contains("UNAVAILABLE") || ex.Message.Contains("503"))
                    {
                        return StatusCode(503, new
                        {
                            success = false,
                            message = "Quiz generation is temporarily unavailable. Please try again in a few minutes.",
                            error = ex.Message
                        });
                    }
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "An unexpected error occurred while generating the quiz.",
                        error = ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred before generating the quiz.",
                    error = ex.Message
                });
            }
        }
    }
}