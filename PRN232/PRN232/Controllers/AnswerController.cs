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
    [Authorize]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAllAnswers()
        {
            try
            {
                var answers = await _answerService.GetAllAnswersAsync();
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving answers", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Answer>> GetAnswer(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerByIdAsync(id);
                if (answer == null)
                    return NotFound(new { message = $"Answer with ID {id} not found" });

                return Ok(answer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the answer", error = ex.Message });
            }
        }

        [HttpGet("question/{questionId}")]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswersByQuestion(int questionId)
        {
            try
            {
                var answers = await _answerService.GetAnswersByQuestionIdAsync(questionId);
                return Ok(answers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving answers for the question", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Answer>> CreateAnswer([FromBody] Answer answer)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdAnswer = await _answerService.CreateAnswerAsync(answer);
                return CreatedAtAction(nameof(GetAnswer), new { id = createdAnswer.Id }, createdAnswer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the answer", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Answer>> UpdateAnswer(int id, [FromBody] Answer answer)
        {
            try
            {
                if (id != answer.Id)
                    return BadRequest(new { message = "Answer ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedAnswer = await _answerService.UpdateAnswerAsync(answer);
                return Ok(updatedAnswer);
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
                return StatusCode(500, new { message = "An error occurred while updating the answer", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnswer(int id)
        {
            try
            {
                var result = await _answerService.DeleteAnswerAsync(id);
                if (!result)
                    return NotFound(new { message = $"Answer with ID {id} not found" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the answer", error = ex.Message });
            }
        }
    }
}
