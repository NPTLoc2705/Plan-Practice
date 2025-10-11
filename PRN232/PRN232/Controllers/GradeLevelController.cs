using BusinessObject.Dtos.LessonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")] // Recommended: Secure for admin use
    public class GradeLevelController : ControllerBase
    {
        private readonly IGradeLevelService _service;

        public GradeLevelController(IGradeLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous] // Allow anyone to view grade levels
        public async Task<IActionResult> GetAll()
        {
            var gradeLevels = await _service.GetAllAsync();
            return Ok(new { success = true, data = gradeLevels });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var gradeLevel = await _service.GetByIdAsync(id);
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
                return BadRequest(new { success = false, errors = ModelState });
            }
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, data = created });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GradeLevelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState });
            }
            try
            {
                var updated = await _service.UpdateAsync(id, request);
                return Ok(new { success = true, data = updated });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { success = true, message = "GradeLevel deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
    }
}
