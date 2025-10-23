using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class ActivityTemplateDAO
    {
        private readonly PlantPraticeDbContext _context;

        public ActivityTemplateDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<ActivityTemplate> GetActivityTemplatesWithDetails(int userId)
        {
            return _context.ActivityTemplates
                .Where(at => at.UserId == userId)
                .Include(at => at.User)
                .OrderBy(at => at.Name);
        }

        public async Task<ActivityTemplate> CreateAsync(ActivityTemplate activityTemplate)
        {
            activityTemplate.CreatedAt = DateTime.UtcNow;
            _context.ActivityTemplates.Add(activityTemplate);
            await _context.SaveChangesAsync();
            return activityTemplate;
        }

        public async Task<ActivityTemplate> GetByIdAsync(int id, int userId)
        {
            return await GetActivityTemplatesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(at => at.Id == id);
        }

        public async Task<List<ActivityTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await GetActivityTemplatesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ActivityTemplate> UpdateAsync(ActivityTemplate activityTemplate)
        {
            _context.ActivityTemplates.Update(activityTemplate);
            await _context.SaveChangesAsync();
            return activityTemplate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var activityTemplate = await _context.ActivityTemplates
                .FirstOrDefaultAsync(at => at.Id == id);

            if (activityTemplate == null) return false;

            _context.ActivityTemplates.Remove(activityTemplate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
