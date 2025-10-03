using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BusinessObject.Lesson;

using Service.Interface;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpPost]
        public async Task<ActionResult<Lesson>> CreateLesson([FromBody] Lesson lesson)
        {
            try
            {
                var createdLesson = await _lessonService.CreateLessonAsync(lesson);
                return CreatedAtAction(nameof(GetLesson), new { id = createdLesson.Id }, createdLesson);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                return Ok(lesson);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Lesson>>> GetAllLessons()
        {
            var lessons = await _lessonService.GetAllLessonsAsync();
            return Ok(lessons);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Lesson>> UpdateLesson(int id, [FromBody] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return BadRequest("Lesson ID mismatch.");
            }

            try
            {
                var updatedLesson = await _lessonService.UpdateLessonAsync(lesson);
                return Ok(updatedLesson);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLesson(int id)
        {
            try
            {
                await _lessonService.DeleteLessonAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}