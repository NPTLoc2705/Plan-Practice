using BusinessObject.Dtos.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.QuizzInterface;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentQuizController : ControllerBase
    {
        private readonly IStudentQuizService _studentQuizService;

        public StudentQuizController(IStudentQuizService studentQuizService)
        {
            _studentQuizService = studentQuizService;
        }

        /// <summary>
        /// Lấy danh sách quiz có thể làm
        /// </summary>
        /// <returns>Danh sách quiz</returns>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<QuizInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableQuizzes()
        {
            try
            {
                var userId = GetCurrentUserId();
                var quizzes = await _studentQuizService.GetAvailableQuizzesAsync(userId);

                return Ok(new
                {
                    success = true,
                    data = quizzes,
                    count = quizzes.Count(),
                    message = "quiz list"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while getting available quizzes",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết quiz để làm bài
        /// </summary>
        /// <param name="quizId">ID của quiz</param>
        /// <returns>Chi tiết quiz với câu hỏi và đáp án</returns>
        [HttpGet("{quizId}")]
        [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuizForTaking(int quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var quiz = await _studentQuizService.GetQuizForTakingAsync(quizId, userId);

                if (quiz == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"not found quiz with id: {quizId}"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = quiz,
                    message = "Get quiz successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while getting quiz",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra học sinh đã làm quiz chưa
        /// </summary>
        /// <param name="quizId">ID của quiz</param>
        /// <returns>True nếu đã làm, False nếu chưa</returns>
        [HttpGet("{quizId}/check-attempt")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckQuizAttempt(int quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var hasAttempted = await _studentQuizService.HasUserAttemptedQuizAsync(userId, quizId);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        quizId = quizId,
                        hasAttempted = hasAttempted
                    },
                    message = hasAttempted
                        ? "You have already taken this quiz"
                        : "You have not taken this quiz yet"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while checking quiz attempt for quiz",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Nộp bài làm quiz
        /// </summary>
        /// <param name="dto">Thông tin bài làm</param>
        /// <returns>Kết quả làm quiz</returns>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(QuizResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data",
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                    });
                }

                var userId = GetCurrentUserId();

                // check user have taken quiz
                var hasAttempted = await _studentQuizService.HasUserAttemptedQuizAsync(userId, dto.QuizId);
                if (hasAttempted)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "You have already taken this quiz. cannot submit again."
                    });
                }

                var result = await _studentQuizService.SubmitQuizAsync(userId, dto);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = $"Submit successfully! Your score: {result.Score}/{result.TotalQuestions}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while submitting quiz",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Xem chi tiết kết quả làm quiz
        /// </summary>
        /// <param name="resultId">ID của kết quả</param>
        /// <returns>Chi tiết kết quả</returns>
        [HttpGet("result/{resultId}")]
        [ProducesResponseType(typeof(QuizResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuizResult(int resultId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _studentQuizService.GetQuizResultAsync(resultId, userId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Cannot find result with ID: {resultId} or you do not have permission to check this result"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Get quiz result successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while getting quiz result",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử làm quiz của học sinh
        /// </summary>
        /// <returns>Danh sách lịch sử làm quiz</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<QuizHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuizHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                var history = await _studentQuizService.GetUserQuizHistoryAsync(userId);

                return Ok(new
                {
                    success = true,
                    data = history,
                    count = history.Count(),
                    message = "Get quiz history successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while getting quiz history",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thống kê quiz của học sinh
        /// </summary>
        /// <param name="quizId">ID của quiz</param>
        /// <returns>Thống kê quiz</returns>
        [HttpGet("{quizId}/statistics")]
        [ProducesResponseType(typeof(QuizStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuizStatistics(int quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var statistics = await _studentQuizService.GetQuizStatisticsAsync(quizId, userId);

                if (statistics == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Cannot find quiz with id: {quizId}"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = statistics,
                    message = "Get quiz statistics successfully"
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while getting quiz statistics for quiz",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Helper method để lấy UserId từ JWT token
        /// </summary>
        /// <returns>User ID</returns>
        private int GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new UnauthorizedAccessException("User ID not found in token");
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    throw new UnauthorizedAccessException("Invalid User ID format");
                }

                return userId;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Unable to get user information", ex);
            }
        }
    }
}

