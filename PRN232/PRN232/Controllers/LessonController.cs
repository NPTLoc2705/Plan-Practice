using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObject.Dtos.LessonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
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

        /// <summary>
        /// Get all lessons for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyLessons()
        {
            try
            {
                var userId = GetCurrentUserId();
                var lessons = await _lessonService.GetLessonsByUserIdAsync(userId);
                return Ok(new { success = true, data = lessons });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving lessons.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific lesson by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var lesson = await _lessonService.GetLessonByIdAsync(id, userId);
                return Ok(new { success = true, data = lesson });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new lesson (UserId is automatically taken from JWT)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LessonRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
                }

                var userId = GetCurrentUserId();
                var created = await _lessonService.CreateLessonAsync(request, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new { success = true, data = created, message = "Lesson created successfully." }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating the lesson.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing lesson
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LessonRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
                }

                var userId = GetCurrentUserId();
                var updated = await _lessonService.UpdateLessonAsync(id, request, userId);

                return Ok(new { success = true, data = updated, message = "Lesson updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the lesson.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a lesson
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _lessonService.DeleteLessonAsync(id, userId);

                if (!success)
                {
                    return NotFound(new { success = false, message = "Lesson not found." });
                }

                return Ok(new { success = true, message = "Lesson deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the lesson.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all lessons (Admin or public access)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLessons()
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonsAsync();
                return Ok(new { success = true, data = lessons });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving lessons.", error = ex.Message });
            }
        }
    }
}