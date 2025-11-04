using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObject.Dtos.LessonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

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

        public LessonPlannerController(
            ILessonPlannerService lessonPlannerService,
            ILessonPlannerDocumentService documentService,
            IWebHostEnvironment environment)
        {
            _lessonPlannerService = lessonPlannerService;
            _documentService = documentService;
            _environment = environment;
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
            var created = await _lessonPlannerService.CreateLessonPlannerAsync(request, userId);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new { success = true, data = created, message = "Lesson planner created successfully." }
            );
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