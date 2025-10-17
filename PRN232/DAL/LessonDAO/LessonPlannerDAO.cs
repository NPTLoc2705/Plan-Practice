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

        // Helper to include all related data for a full lesson plan view.
        private IQueryable<LessonPlanner> GetLessonPlannersWithDetails()
        {
            return _context.LessonPlanners
                .Include(lp => lp.Class).ThenInclude(c => c.GradeLevel)
                .Include(lp => lp.Unit)
                .Include(lp => lp.LessonDefinition)
                .Include(lp => lp.MethodTemplate)
                .Include(lp => lp.Objectives)
                .Include(lp => lp.Skills)
                .Include(lp => lp.Attitudes)
                .Include(lp => lp.LanguageFocusItems)
                .Include(lp => lp.Preparations)
                .Include(lp => lp.ActivityStages).ThenInclude(s => s.ActivityItems);
        }

        public async Task<LessonPlanner> CreateLessonPlannerAsync(LessonPlanner lessonPlanner)
        {
            lessonPlanner.CreatedAt = DateTime.UtcNow;
            _context.LessonPlanners.Add(lessonPlanner);
            await _context.SaveChangesAsync();
            return lessonPlanner;
        }

        public async Task<LessonPlanner> GetLessonPlannerByIdAsync(int id)
        {
            return await GetLessonPlannersWithDetails().AsNoTracking().FirstOrDefaultAsync(lp => lp.Id == id);
        }

        public async Task<List<LessonPlanner>> GetAllLessonPlannersAsync()
        {
            // Note: Not using GetLessonPlannersWithDetails for list views to avoid performance issues.
            // Only essential data is loaded.
            return await _context.LessonPlanners
                .Include(lp => lp.Class)
                .Include(lp => lp.Unit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<LessonPlanner>> GetLessonPlannersByUserIdAsync(int userId)
        {
            return await _context.LessonPlanners
                .Where(l => l.UserId == userId)
                .Include(lp => lp.Class)
                .Include(lp => lp.Unit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LessonPlanner> UpdateLessonPlannerAsync(LessonPlanner lessonPlanner)
        {
            lessonPlanner.UpdatedAt = DateTime.UtcNow;

            // The service layer will manage adding/updating/deleting children.
            // Here, we just persist the changes to the tracked entity.
            _context.LessonPlanners.Update(lessonPlanner);
            await _context.SaveChangesAsync();
            return lessonPlanner;
        }

        public async Task<bool> DeleteLessonPlannerAsync(int id)
        {
            // Loading with all children ensures that dependent data is also removed
            // if cascading deletes are configured. This is a safer approach.
            var lessonPlanner = await _context.LessonPlanners
                .Include(lp => lp.ActivityStages).ThenInclude(s => s.ActivityItems)
                .Include(lp => lp.Objectives)
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (lessonPlanner == null) return false;

            _context.LessonPlanners.Remove(lessonPlanner);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}