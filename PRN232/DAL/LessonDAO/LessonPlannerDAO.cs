using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject;
using BusinessObject.Lesson;
using Microsoft.EntityFrameworkCore;

namespace DAL.LessonDAO
{
    public class LessonPlannerDAO
    {
        private readonly PlantPraticeDbContext _context;

        public LessonPlannerDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        // Helper to include related data
        private IQueryable<LessonPlanner> GetLessonPlannersWithDetails()
        {
            return _context.LessonPlanners
                .Include(lp => lp.Class)
                .ThenInclude(c => c.GradeLevel);
        }

        public async Task<LessonPlanner> CreateLessonPlannerAsync(LessonPlanner lessonPlanner)
        {
            _context.LessonPlanners.Add(lessonPlanner);
            await _context.SaveChangesAsync();
            return lessonPlanner;
        }

        public async Task<LessonPlanner> GetLessonPlannerByIdAsync(int id)
        {
            return await GetLessonPlannersWithDetails().FirstOrDefaultAsync(lp => lp.Id == id);
        }

        public async Task<List<LessonPlanner>> GetAllLessonPlannersAsync()
        {
            return await GetLessonPlannersWithDetails().ToListAsync();
        }

        public async Task<List<LessonPlanner>> GetLessonPlannersByUserIdAsync(int userId)
        {
            return await GetLessonPlannersWithDetails()
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<LessonPlanner> UpdateLessonPlannerAsync(LessonPlanner lessonPlanner)
        {
            _context.Entry(lessonPlanner).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return lessonPlanner;
        }

        public async Task<bool> DeleteLessonPlannerAsync(int id)
        {
            var lessonPlanner = await _context.LessonPlanners.FindAsync(id);
            if (lessonPlanner == null) return false;

            _context.LessonPlanners.Remove(lessonPlanner);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}