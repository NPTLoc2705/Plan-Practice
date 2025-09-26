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
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAllQuestions()
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync();
                var questionDtos = questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    QuizId = q.QuizId,

                });
                return Ok(questionDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving questions", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                    return NotFound(new { message = $"Question with ID {id} not found" });

                var questionDto = new QuestionDto
                {
                    Id = question.Id,
                    Content = question.Content,
                    QuizId = question.QuizId,
                
                };
                return Ok(questionDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the question", error = ex.Message });
            }
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByQuiz(int quizId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
                var questionDtos = questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    QuizId = q.QuizId,
                   
                });
                return Ok(questionDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving questions for the quiz", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] QuestionDto questionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var question = new Question
                {
                    Content = questionDto.Content,
                    QuizId = questionDto.QuizId
                };

                await _questionService.CreateQuestionAsync(question);
                
                return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the question", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDto>> UpdateQuestion(int id, [FromBody] QuestionDto questionDto)
        {
            try
            {
                if (id != questionDto.Id)
                    return BadRequest(new { message = "Question ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var question = new Question
                {
                    Id = questionDto.Id,
                    Content = questionDto.Content,
                    QuizId = questionDto.QuizId
                };

                await _questionService.UpdateQuestionAsync(question);
              
                return Ok();
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
                return StatusCode(500, new { message = "An error occurred while updating the question", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the question", error = ex.Message });
            }
        }
    }
}