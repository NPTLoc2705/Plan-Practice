using BusinessObject.Dtos.LessonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GradeLevelController : ControllerBase
    {
        private readonly IGradeLevelService _gradeLevelService;
        public GradeLevelController(IGradeLevelService gradeLevelService)
        {
            _gradeLevelService = gradeLevelService;
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
        [HttpGet("my-grade-levels")]
        public async Task<IActionResult> GetMyGradeLevels()
        {
            var userId = GetCurrentUserId();
            var gradeLevels = await _gradeLevelService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = gradeLevels });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var gradeLevel = await _gradeLevelService.GetByIdAsync(id, userId);
                return Ok(new { success = true, data = gradeLevel });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GradeLevelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var created = await _gradeLevelService.CreateAsync(request, userId);
                return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new { success = true, data = created, message = "Grade level created successfully." }
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GradeLevelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _gradeLevelService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Grade level updated successfully." });
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
                var success = await _gradeLevelService.DeleteAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Grade level not found." });
                }
                return Ok(new { success = true, message = "Grade level deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            var gradeLevels = await _gradeLevelService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = gradeLevels });
        }
    }
}
