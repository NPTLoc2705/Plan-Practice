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
    
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerDto>>> GetAllAnswers()
        {
            try
            {
                var answers = await _answerService.GetAllAnswersAsync();
                var answerDtos = answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    QuestionId = a.QuestionId
                });
                return Ok(answerDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving answers", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerDto>> GetAnswer(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerByIdAsync(id);
                if (answer == null)
                    return NotFound(new { message = $"Answer with ID {id} not found" });

                var answerDto = new AnswerDto
                {
                    Id = answer.Id,
                    Content = answer.Content,
                    IsCorrect = answer.IsCorrect,
                    QuestionId = answer.QuestionId
                };
                return Ok(answerDto);
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
        public async Task<ActionResult<IEnumerable<AnswerDto>>> GetAnswersByQuestion(int questionId)
        {
            try
            {
                var answers = await _answerService.GetAnswersByQuestionIdAsync(questionId);
                var answerDtos = answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    QuestionId = a.QuestionId
                });
                return Ok(answerDtos);
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
        public async Task<ActionResult<AnswerDto>> CreateAnswer([FromBody] AnswerDto answerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var answer = new Answer
                {
                    Content = answerDto.Content,
                    IsCorrect = answerDto.IsCorrect,
                    QuestionId = answerDto.QuestionId
                };

                await _answerService.CreateAnswerAsync(answer);
              
                return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
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
        public async Task<ActionResult<AnswerDto>> UpdateAnswer(int id, [FromBody] AnswerDto answerDto)
        {
            try
            {
                if (id != answerDto.Id)
                    return BadRequest(new { message = "Answer ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var answer = new Answer
                {
                    Id = answerDto.Id,
                    Content = answerDto.Content,
                    IsCorrect = answerDto.IsCorrect,
                    QuestionId = answerDto.QuestionId
                };

                 await _answerService.UpdateAnswerAsync(answer);
                
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
                return StatusCode(500, new { message = "An error occurred while updating the answer", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnswer(int id)
        {
            try
            {
                await _answerService.DeleteAnswerAsync(id);
              

                return Ok();
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