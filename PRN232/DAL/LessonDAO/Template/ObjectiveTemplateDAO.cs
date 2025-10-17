using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class ObjectiveTemplateDAO
    {
        private readonly PlantPraticeDbContext _context;

        public ObjectiveTemplateDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<ObjectiveTemplate> GetObjectiveTemplatesWithDetails(int userId)
        {
            return _context.ObjectiveTemplates
                .Where(ot => ot.UserId == userId)
                .Include(ot => ot.User)
                .OrderBy(ot => ot.DisplayOrder)
                .ThenBy(ot => ot.Name);
        }

        public async Task<ObjectiveTemplate> CreateAsync(ObjectiveTemplate objectiveTemplate)
        {
            objectiveTemplate.CreatedAt = DateTime.UtcNow;
            _context.ObjectiveTemplates.Add(objectiveTemplate);
            await _context.SaveChangesAsync();
            return objectiveTemplate;
        }

        public async Task<ObjectiveTemplate> GetByIdAsync(int id, int userId)
        {
            return await GetObjectiveTemplatesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(ot => ot.Id == id);
        }

        public async Task<List<ObjectiveTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await GetObjectiveTemplatesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObjectiveTemplate> UpdateAsync(ObjectiveTemplate objectiveTemplate)
        {
            _context.ObjectiveTemplates.Update(objectiveTemplate);
            await _context.SaveChangesAsync();
            return objectiveTemplate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var objectiveTemplate = await _context.ObjectiveTemplates
                .FirstOrDefaultAsync(ot => ot.Id == id);

            if (objectiveTemplate == null) return false;

            _context.ObjectiveTemplates.Remove(objectiveTemplate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
