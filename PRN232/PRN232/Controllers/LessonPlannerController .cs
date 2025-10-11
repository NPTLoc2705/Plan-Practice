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
    public class LessonPlannerController : ControllerBase
    {
        private readonly ILessonPlannerService _lessonPlannerService;

        public LessonPlannerController(ILessonPlannerService lessonPlannerService)
        {
            _lessonPlannerService = lessonPlannerService;
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
    }
}