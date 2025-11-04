using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObject.Dtos.LessonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonPlannerDocumentController : ControllerBase
    {
        private readonly ILessonPlannerDocumentService _documentService;
        private readonly IWebHostEnvironment _environment;

        public LessonPlannerDocumentController(
            ILessonPlannerDocumentService documentService,
            IWebHostEnvironment environment)
        {
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

        [HttpGet("lesson/{lessonPlannerId}")]
        public async Task<IActionResult> GetDocumentsByLessonPlannerId(int lessonPlannerId)
        {
            try
            {
                var documents = await _documentService.GetDocumentsByLessonPlannerIdAsync(lessonPlannerId);
                return Ok(new { success = true, data = documents });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                return Ok(new { success = true, data = document });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] LessonPlannerDocumentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }

            try
            {
                var created = await _documentService.CreateDocumentAsync(request);
                return CreatedAtAction(
                    nameof(GetDocumentById),
                    new { id = created.Id },
                    new { success = true, data = created, message = "Document record created successfully." }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                
                // Delete the physical file if it exists
                var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                var success = await _documentService.DeleteDocumentAsync(id);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Document not found." });
                }

                return Ok(new { success = true, message = "Document deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { success = false, message = "File not found on server." });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = Path.GetFileName(filePath);

                return File(fileBytes, "application/msword", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
