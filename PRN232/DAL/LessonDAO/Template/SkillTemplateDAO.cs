using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class SkillTemplateDAO
    {
        private readonly PlantPraticeDbContext _context;

        public SkillTemplateDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<SkillTemplate> GetSkillTemplatesWithDetails(int userId)
        {
            return _context.SkillTemplates
                .Where(st => st.UserId == userId)
                .Include(st => st.User)
                .OrderBy(st => st.DisplayOrder)
                .ThenBy(st => st.Name);
        }

        public async Task<SkillTemplate> CreateAsync(SkillTemplate skillTemplate)
        {
            skillTemplate.CreatedAt = DateTime.UtcNow;
            _context.SkillTemplates.Add(skillTemplate);
            await _context.SaveChangesAsync();
            return skillTemplate;
        }

        public async Task<SkillTemplate> GetByIdAsync(int id, int userId)
        {
            return await GetSkillTemplatesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<List<SkillTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await GetSkillTemplatesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SkillTemplate> UpdateAsync(SkillTemplate skillTemplate)
        {
            _context.SkillTemplates.Update(skillTemplate);
            await _context.SaveChangesAsync();
            return skillTemplate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var skillTemplate = await _context.SkillTemplates
                .FirstOrDefaultAsync(st => st.Id == id);

            if (skillTemplate == null) return false;

            _context.SkillTemplates.Remove(skillTemplate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
