using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using Microsoft.EntityFrameworkCore;
namespace DAL.LessonDAO
{
    public class LessonDAO
    {
        private readonly PlantPraticeDbContext _context;
        public LessonDAO(PlantPraticeDbContext content)
        {
            _context = content;
        }
        public async Task<LessonResponse> CreateLesson(LessonRequest lesson)
        {
            var entity = MapToEntity(lesson);
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return MapToEntityResponse(entity);
        }

        public async Task<LessonResponse> UpdateLesson(LessonRequest lesson)
        {
            var exist = await _context.Lessons.FindAsync(lesson.Id);
            if (exist == null)
            {
                throw new Exception($"Lesson with ID {lesson.Id} not found.");
            }
            exist.Title = lesson.Title;
            exist.Content = lesson.Content;
            exist.GradeLevel = lesson.GradeLevel;
            exist.Description = lesson.Description;
            exist.UserId = lesson.UserId;

            await _context.SaveChangesAsync();
            return MapToEntityResponse(exist);

        }

        public async Task<List<LessonResponse>> ViewLesson(int UserId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.UserId == UserId)
                .ToListAsync();

            if (!lessons.Any())
            {
                throw new Exception($"{UserId} doesn't have any lesson bro");
            }

            return lessons.Select(MapToEntityResponse).ToList();
        }

        public async Task<bool> DeleteLesson(int Id)
        {
            var exist = await _context.Lessons.FindAsync(Id);
            if(exist == null)
            {
               return false;
            }
            _context.Lessons.Remove(exist);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<LessonResponse>> ViewAllLesson()
        {
            var lessons = await _context.Lessons.ToListAsync();
            return lessons.Select(MapToEntityResponse).ToList();
        }

        private Lesson MapToEntity(LessonRequest request)
        {
            return new Lesson
            {
                Id = request.Id,
                Title = request.Title,
                Content = request.Content,
                GradeLevel = request.GradeLevel,
                Description = request.Description,
                UserId = request.UserId
            };
        }
        private LessonResponse MapToEntityResponse(Lesson request)
        {
            return new LessonResponse
            {
                Id = request.Id,
                Title = request.Title,
                Content = request.Content,
                GradeLevel = request.GradeLevel,
                Description = request.Description,
            };

        }

    }
}
