using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class AttitudeTemplateDAO
    {
        private readonly PlantPraticeDbContext _context;

        public AttitudeTemplateDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<AttitudeTemplate> GetAttitudeTemplatesWithDetails(int userId)
        {
            return _context.AttitudeTemplates
                .Where(at => at.UserId == userId)
                .Include(at => at.User)
                .OrderBy(at => at.DisplayOrder);
        }

        public async Task<AttitudeTemplate> CreateAsync(AttitudeTemplate attitudeTemplate)
        {
            attitudeTemplate.CreatedAt = DateTime.UtcNow;
            _context.AttitudeTemplates.Add(attitudeTemplate);
            await _context.SaveChangesAsync();
            return attitudeTemplate;
        }

        public async Task<AttitudeTemplate> GetByIdAsync(int id, int userId)
        {
            return await GetAttitudeTemplatesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(at => at.Id == id);
        }

        public async Task<List<AttitudeTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await GetAttitudeTemplatesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AttitudeTemplate> UpdateAsync(AttitudeTemplate attitudeTemplate)
        {
            _context.AttitudeTemplates.Update(attitudeTemplate);
            await _context.SaveChangesAsync();
            return attitudeTemplate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var attitudeTemplate = await _context.AttitudeTemplates
                .FirstOrDefaultAsync(at => at.Id == id);

            if (attitudeTemplate == null) return false;

            _context.AttitudeTemplates.Remove(attitudeTemplate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
