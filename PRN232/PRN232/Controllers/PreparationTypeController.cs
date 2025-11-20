using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.Template;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/Preparation")]
    [Authorize]
    public class PreparationTypeController : ControllerBase
    {
        private readonly IPreparationTypeService _preparationTypeService;

        public PreparationTypeController(IPreparationTypeService preparationTypeService)
        {
            _preparationTypeService = preparationTypeService;
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
            var types = await _preparationTypeService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = types });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var type = await _preparationTypeService.GetByIdAsync(id, userId);
                return Ok(new { success = true, data = type });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PreparationTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var created = await _preparationTypeService.CreateAsync(request, userId);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new { success = true, data = created, message = "Preparation type created successfully." }
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PreparationTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _preparationTypeService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Preparation type updated successfully." });
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
                var success = await _preparationTypeService.DeleteAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Preparation type not found." });
                }
                return Ok(new { success = true, message = "Preparation type deleted successfully." });
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
            var types = await _preparationTypeService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = types });
        }
    }
}
