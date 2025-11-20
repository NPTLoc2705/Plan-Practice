using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.Template;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/Objective/template")]
    [Authorize]
    public class ObjectiveTemplateController : ControllerBase
    {
        private readonly IObjectiveTemplateService _objectiveTemplateService;

        public ObjectiveTemplateController(IObjectiveTemplateService objectiveTemplateService)
        {
            _objectiveTemplateService = objectiveTemplateService;
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

        [HttpGet("my-templates")]
        public async Task<IActionResult> GetMyTemplates()
        {
            var userId = GetCurrentUserId();
            var templates = await _objectiveTemplateService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = templates });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var template = await _objectiveTemplateService.GetByIdAsync(id, userId);
                return Ok(new { success = true, data = template });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ObjectiveTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var created = await _objectiveTemplateService.CreateAsync(request, userId);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new { success = true, data = created, message = "Objective template created successfully." }
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ObjectiveTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data.", errors = ModelState });
            }
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _objectiveTemplateService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated, message = "Objective template updated successfully." });
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
                var success = await _objectiveTemplateService.DeleteAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Objective template not found." });
                }
                return Ok(new { success = true, message = "Objective template deleted successfully." });
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
            var templates = await _objectiveTemplateService.GetAllByUserIdAsync(userId);
            return Ok(new { success = true, data = templates });
        }
    }
}
