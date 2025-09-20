using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.QuizzInterface;


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
        public async Task<ActionResult<IEnumerable<Question>>> GetAllQuestions()
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving questions", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                    return NotFound(new { message = $"Question with ID {id} not found" });

                return Ok(question);
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
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByQuiz(int quizId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
                return Ok(questions);
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
        public async Task<ActionResult<Question>> CreateQuestion([FromBody] Question question)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdQuestion = await _questionService.CreateQuestionAsync(question);
                return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
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
        public async Task<ActionResult<Question>> UpdateQuestion(int id, [FromBody] Question question)
        {
            try
            {
                if (id != question.Id)
                    return BadRequest(new { message = "Question ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedQuestion = await _questionService.UpdateQuestionAsync(question);
                return Ok(updatedQuestion);
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
                var result = await _questionService.DeleteQuestionAsync(id);
                if (!result)
                    return NotFound(new { message = $"Question with ID {id} not found" });

                return NoContent();
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
