using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessObject.Lesson;

namespace DAL.LessonDAO
{
    public class LessonDAO
    {
        private readonly PlantPraticeDbContext _context;

        public LessonDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            return await _context.Lessons.FindAsync(id);
        }

        public async Task<List<Lesson>> GetAllLessonsAsync()
        {
            return await _context.Lessons.ToListAsync();
        }

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task DeleteLessonAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
        }
    }
}
