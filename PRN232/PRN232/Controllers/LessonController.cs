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
    [Route("api/lesson")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user ID." });
            }

            var lessons = await _lessonService.GetLessonsByUserIdAsync(userId);
            return Ok(lessons);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] LessonRequest lesson)
        {
            if (lesson == null)
                return BadRequest(new { Message = "Lesson cannot be null" });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user ID." });
            }

            if (lesson.UserId != userId)
                return Unauthorized(new { Message = "Cannot create lesson for another user" });

            var created = await _lessonService.CreateLessonAsync(lesson, userId);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] LessonRequest lesson)
        {
            if (lesson == null)
                return BadRequest(new { Message = "Invalid lesson data" });

            if (lesson.Id != id)
                return BadRequest(new { Message = "Lesson ID mismatch" });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user ID." });
            }

            if (lesson.UserId != userId)
                return Unauthorized(new { Message = "Cannot update lesson for another user" });

            var updated = await _lessonService.UpdateLessonAsync(lesson, userId);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user ID." });
            }

            var success = await _lessonService.DeleteLessonAsync(id, userId);
            if (!success)
                return NotFound(new { Message = "Lesson not found" });

            return Ok(new { Message = "Lesson deleted successfully" });
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllLessons()
        {
            var lessons = await _lessonService.GetAllLessonsAsync();
            return Ok(lessons);
        }
    }
}