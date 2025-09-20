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
    
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetAllQuizzes()
        {
            try
            {
                var quizzes = await _quizService.GetAllQuizzesAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving quizzes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuiz(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz == null)
                    return NotFound(new { message = $"Quiz with ID {id} not found" });

                return Ok(quiz);
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
        public async Task<ActionResult<Quiz>> CreateQuiz([FromBody] Quiz quiz)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdQuiz = await _quizService.CreateQuizAsync(quiz);
                return CreatedAtAction(nameof(GetQuiz), new { id = createdQuiz.Id }, createdQuiz);
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
        public async Task<ActionResult<Quiz>> UpdateQuiz(int id, [FromBody] Quiz quiz)
        {
            try
            {
                if (id != quiz.Id)
                    return BadRequest(new { message = "Quiz ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedQuiz = await _quizService.UpdateQuizAsync(quiz);
                return Ok(updatedQuiz);
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
