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
    public class LanguageFocusTypeController : ControllerBase
    {
        private readonly ILanguageFocusTypeService _languageFocusTypeService;

        public LanguageFocusTypeController(ILanguageFocusTypeService languageFocusTypeService)
        {
            _languageFocusTypeService = languageFocusTypeService;
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

        [HttpGet("my-types")]
        public async Task<IActionResult> GetMyTypes()
        {
            var userId = GetCurrentUserId();
            var types = await _languageFocusTypeService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = types });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var type = await _languageFocusTypeService.GetByIdAsync(id, userId);
                return Ok(new { success = true, data = type });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LanguageFocusTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var created = await _languageFocusTypeService.CreateAsync(request, userId);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new { success = true, data = created, message = "Language focus type created successfully." }
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LanguageFocusTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _languageFocusTypeService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Language focus type updated successfully." });
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
                var success = await _languageFocusTypeService.DeleteAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Language focus type not found." });
                }
                return Ok(new { success = true, message = "Language focus type deleted successfully." });
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
            var types = await _languageFocusTypeService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = types });
        }
    }
}
