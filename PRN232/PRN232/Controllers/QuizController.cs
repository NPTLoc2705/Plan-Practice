using BusinessObject.Dtos;
using BusinessObject.Quiz;
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
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
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
                    Questions = q.Questions.Select(ques => new QuestionDto
                    {
                        Id = ques.Id,
                        Content = ques.Content,
                        QuizId = ques.QuizId,
                        Answers = ques.Answers.Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList(),
                    QuizResults = q.QuizResults.Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        QuizId = r.QuizId,
                        Score = r.Score,
                        CompletedAt = r.CompletedAt,
                        UserAnswers = r.UserAnswers.Select(ua => new UserAnswerDto
                        {
                            Id = ua.Id,
                            QuizResultId = ua.QuizResultId,
                            QuestionId = ua.QuestionId,
                            AnswerId = ua.AnswerId
                        }).ToList()
                    }).ToList()
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
                    Questions = quiz.Questions.Select(ques => new QuestionDto
                    {
                        Id = ques.Id,
                        Content = ques.Content,
                        QuizId = ques.QuizId,
                        Answers = ques.Answers.Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList(),
                    QuizResults = quiz.QuizResults.Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        QuizId = r.QuizId,
                        Score = r.Score,
                        CompletedAt = r.CompletedAt,
                        UserAnswers = r.UserAnswers.Select(ua => new UserAnswerDto
                        {
                            Id = ua.Id,
                            QuizResultId = ua.QuizResultId,
                            QuestionId = ua.QuestionId,
                            AnswerId = ua.AnswerId
                        }).ToList()
                    }).ToList()
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

        [HttpPost]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] QuizDto quizDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quiz = new Quiz
                {
                    Title = quizDto.Title,
                    Description = quizDto.Description,
                    CreatedAt = quizDto.CreatedAt
                };

                var createdQuiz = await _quizService.CreateQuizAsync(quiz);
                var createdDto = new QuizDto
                {
                    Id = createdQuiz.Id,
                    Title = createdQuiz.Title,
                    Description = createdQuiz.Description,
                    CreatedAt = createdQuiz.CreatedAt,
                    Questions = createdQuiz.Questions.Select(ques => new QuestionDto
                    {
                        Id = ques.Id,
                        Content = ques.Content,
                        QuizId = ques.QuizId,
                        Answers = ques.Answers.Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList(),
                    QuizResults = createdQuiz.QuizResults.Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        QuizId = r.QuizId,
                        Score = r.Score,
                        CompletedAt = r.CompletedAt,
                        UserAnswers = r.UserAnswers.Select(ua => new UserAnswerDto
                        {
                            Id = ua.Id,
                            QuizResultId = ua.QuizResultId,
                            QuestionId = ua.QuestionId,
                            AnswerId = ua.AnswerId
                        }).ToList()
                    }).ToList()
                };
                return CreatedAtAction(nameof(GetQuiz), new { id = createdDto.Id }, createdDto);
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
        public async Task<ActionResult<QuizDto>> UpdateQuiz(int id, [FromBody] QuizDto quizDto)
        {
            try
            {
                if (id != quizDto.Id)
                    return BadRequest(new { message = "Quiz ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quiz = new Quiz
                {
                    Id = quizDto.Id,
                    Title = quizDto.Title,
                    Description = quizDto.Description,
                    CreatedAt = quizDto.CreatedAt
                };

                var updatedQuiz = await _quizService.UpdateQuizAsync(quiz);
                var updatedDto = new QuizDto
                {
                    Id = updatedQuiz.Id,
                    Title = updatedQuiz.Title,
                    Description = updatedQuiz.Description,
                    CreatedAt = updatedQuiz.CreatedAt,
                    Questions = updatedQuiz.Questions.Select(ques => new QuestionDto
                    {
                        Id = ques.Id,
                        Content = ques.Content,
                        QuizId = ques.QuizId,
                        Answers = ques.Answers.Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList(),
                    QuizResults = updatedQuiz.QuizResults.Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        QuizId = r.QuizId,
                        Score = r.Score,
                        CompletedAt = r.CompletedAt,
                        UserAnswers = r.UserAnswers.Select(ua => new UserAnswerDto
                        {
                            Id = ua.Id,
                            QuizResultId = ua.QuizResultId,
                            QuestionId = ua.QuestionId,
                            AnswerId = ua.AnswerId
                        }).ToList()
                    }).ToList()
                };
                return Ok(updatedDto);
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
        public async Task<ActionResult> DeleteQuiz(int id)
        {
            try
            {
                var result = await _quizService.DeleteQuizAsync(id);
                if (!result)
                    return NotFound(new { message = $"Quiz with ID {id} not found" });

                return NoContent();
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
    }
}