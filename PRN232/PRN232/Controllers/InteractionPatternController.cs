using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.Template;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InteractionPatternController : ControllerBase
    {
        private readonly IInteractionPatternService _interactionPatternService;

        public InteractionPatternController(IInteractionPatternService interactionPatternService)
        {
            _interactionPatternService = interactionPatternService;
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

        [HttpGet("my-patterns")]
        public async Task<IActionResult> GetMyPatterns()
        {
            var userId = GetCurrentUserId();
            var patterns = await _interactionPatternService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = patterns });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var pattern = await _interactionPatternService.GetByIdAsync(id, userId);
                return Ok(new { success = true, data = pattern });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InteractionPatternRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var created = await _interactionPatternService.CreateAsync(request, userId);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new { success = true, data = created, message = "Interaction pattern created successfully." }
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InteractionPatternRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _interactionPatternService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Interaction pattern updated successfully." });
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
                var success = await _interactionPatternService.DeleteAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Interaction pattern not found." });
                }
                return Ok(new { success = true, message = "Interaction pattern deleted successfully." });
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
            var patterns = await _interactionPatternService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = patterns });
        }
    }
}
