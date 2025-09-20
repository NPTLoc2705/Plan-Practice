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
    
    public class QuizManagementController : ControllerBase
    {
        private readonly IQuizManagementService _quizManagementService;

        public QuizManagementController(IQuizManagementService quizManagementService)
        {
            _quizManagementService = quizManagementService;
        }

        [HttpGet("quiz/{quizId}/details")]
        public async Task<ActionResult<QuizWithDetails>> GetQuizWithDetails(int quizId)
        {
            try
            {
                var quizWithDetails = await _quizManagementService.GetQuizWithQuestionsAndAnswersAsync(quizId);
                return Ok(quizWithDetails);
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
        public async Task<ActionResult<QuizWithDetails>> CreateCompleteQuiz([FromBody] CreateQuizRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quizWithDetails = await _quizManagementService.CreateCompleteQuizAsync(request);
                return CreatedAtAction(nameof(GetQuizWithDetails), new { quizId = quizWithDetails.Quiz.Id }, quizWithDetails);
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
        public async Task<ActionResult<QuizResult>> SubmitQuizAnswers([FromBody] SubmitQuizRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _quizManagementService.SubmitQuizAnswersAsync(request);
                return Ok(result);
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
        public async Task<ActionResult<IEnumerable<QuizResult>>> GetUserQuizHistory(int userId)
        {
            try
            {
                var history = await _quizManagementService.GetUserQuizHistoryAsync(userId);
                return Ok(history);
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
