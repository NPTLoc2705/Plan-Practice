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
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        public ClassController(IClassService classService)
        {
            _classService = classService;
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
        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            var userId = GetCurrentUserId();
            var classes = await _classService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = classes });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var aClass = await _classService.GetByIdAsync(id, userId);
                return Ok(new { success = true, data = aClass });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var created = await _classService.CreateAsync(request, userId);
                return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new { success = true, data = created, message = "Class created successfully." }
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _classService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Class updated successfully." });
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
                var success = await _classService.DeleteAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Class not found." });
                }
                return Ok(new { success = true, message = "Class deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("by-grade/{gradeLevelId}")]
        public async Task<IActionResult> GetAllByGradeLevel(int gradeLevelId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var classes = await _classService.GetAllByGradeLevelIdAsync(gradeLevelId, userId);
                return Ok(new { success = true, data = classes });
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
            var classes = await _classService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = classes });
        }
    }
}
