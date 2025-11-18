using BusinessObject.Dtos.LessonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Service.Method;
using Service.QuizzInterface;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonPlannerController : ControllerBase
    {
        private readonly ILessonPlannerService _lessonPlannerService;
        private readonly ILessonPlannerDocumentService _documentService;
        private readonly IWebHostEnvironment _environment;
        private readonly ICoinService _coinService;
        private readonly IGeminiService _geminiService;
        private const int LessonGenerationCoinCost = 50;

        public LessonPlannerController(
            ILessonPlannerService lessonPlannerService,
            ILessonPlannerDocumentService documentService,
            IWebHostEnvironment environment,
            ICoinService coinService,
            IGeminiService geminiService)
        {
            _lessonPlannerService = lessonPlannerService;
            _documentService = documentService;
            _environment = environment;
            _coinService = coinService;
            _geminiService = geminiService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token.");
            }
            return userId;
        }

        [HttpGet("coin-balance")]
        public async Task<IActionResult> GetCoinBalance()
        {
            try
            {
                var userId = GetCurrentUserId();
                var balance = await _coinService.GetUserCoinBalance(userId);

                return Ok(new
                {
                    success = true,
                    coinBalance = balance
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error getting coin balance" });
            }
        }


        [HttpGet("my-planners")]
        public async Task<IActionResult> GetMyLessonPlanners()
        {
            var userId = GetCurrentUserId();
            var planners = await _lessonPlannerService.GetLessonPlannersByUserIdAsync(userId);
            return Ok(new { success = true, data = planners });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var planner = await _lessonPlannerService.GetLessonPlannerByIdAsync(id);
                return Ok(new { success = true, data = planner });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LessonPlannerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            var userId = GetCurrentUserId();
            LessonPlannerResponse created = null;

            try
            {
                var currentBalance = await _coinService.GetUserCoinBalance(userId);
                if (currentBalance < LessonGenerationCoinCost)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Insufficient coins. You need {LessonGenerationCoinCost} coins to save a lesson plan.",
                        coinBalance = currentBalance
                    });
                }

                created = await _lessonPlannerService.CreateLessonPlannerAsync(request, userId);

                var deductionSucceeded = await _coinService.DeductCoinsForLessonGeneration(userId);
                if (!deductionSucceeded)
                {
                    await _lessonPlannerService.DeleteLessonPlannerAsync(created.Id, userId);
                    var refreshedBalance = await _coinService.GetUserCoinBalance(userId);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Unable to deduct coins. Please refresh your balance and try again.",
                        coinBalance = refreshedBalance
                    });
                }

                var newBalance = await _coinService.GetUserCoinBalance(userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new
                    {
                        success = true,
                        data = created,
                        newBalance,
                        message = "Lesson planner created successfully."
                    }
                );
            }
            catch (Exception ex)
            {
                if (created != null)
                {
                    try
                    {
                        await _lessonPlannerService.DeleteLessonPlannerAsync(created.Id, userId);
                    }
                    catch
                    {
                        // Suppress secondary errors during cleanup to surface the original failure.
                    }
                }

                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while creating the lesson planner.",
                    detail = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LessonPlannerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _lessonPlannerService.UpdateLessonPlannerAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Lesson planner updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _lessonPlannerService.DeleteLessonPlannerAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Lesson planner not found." });
                }
                return Ok(new { success = true, message = "Lesson planner deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all")]
        [AllowAnonymous] // Or use Admin role authorization
        public async Task<IActionResult> GetAll()
        {
            var planners = await _lessonPlannerService.GetAllLessonPlannersAsync();
            return Ok(new { success = true, data = planners });
        }

        /// <summary>
        /// Generate a lesson planner using AI (Gemini)
        /// </summary>
        [HttpPost("generate-ai")]
        public async Task<IActionResult> GenerateLessonPlannerWithAI([FromBody] GenerateLessonPlannerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }

            try
            {
                var userId = GetCurrentUserId();

                // Check if user has enough coins
                var currentBalance = await _coinService.GetUserCoinBalance(userId);
                if (currentBalance < LessonGenerationCoinCost)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Insufficient coins. You need {LessonGenerationCoinCost} coins but only have {currentBalance}.",
                        requiredCoins = LessonGenerationCoinCost,
                        currentBalance = currentBalance
                    });
                }

                // Generate lesson planner using AI
                var generatedLesson = await _geminiService.GenerateLessonPlannerAsync(request);

                // Deduct coins
                await _coinService.DeductCoinsForLessonGeneration(userId);

                // Get updated balance
                var newBalance = await _coinService.GetUserCoinBalance(userId);

                return Ok(new
                {
                    success = true,
                    data = generatedLesson,
                    coinsDeducted = LessonGenerationCoinCost,
                    newBalance = newBalance,
                    message = "Lesson planner generated successfully!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error generating lesson planner: {ex.Message}"
                });
            }
        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] int lessonPlannerId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded." });
            }

            try
            {
                // Create documents directory if it doesn't exist
                var documentsPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "documents");
                if (!Directory.Exists(documentsPath))
                {
                    Directory.CreateDirectory(documentsPath);
                }

                // Save file to server
                var fileName = file.FileName;
                var filePath = Path.Combine(documentsPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create document record in database
                var documentRequest = new LessonPlannerDocumentRequest
                {
                    FilePath = $"/documents/{fileName}",
                    LessonPlannerId = lessonPlannerId
                };

                var createdDocument = await _documentService.CreateDocumentAsync(documentRequest);

                return Ok(new { 
                    success = true, 
                    data = createdDocument,
                    message = "Document uploaded and saved successfully." 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Upload failed: {ex.Message}" });
            }
        }
    }
}