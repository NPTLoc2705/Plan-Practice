using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class MethodTemplateDAO
    {
        private readonly PlantPraticeDbContext _context;

        public MethodTemplateDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<MethodTemplate> GetMethodTemplatesWithDetails(int userId)
        {
            return _context.MethodTemplates
                .Where(mt => mt.UserId == userId)
                .Include(mt => mt.User)
                .OrderBy(mt => mt.Name);
        }

        public async Task<MethodTemplate> CreateAsync(MethodTemplate methodTemplate)
        {
            methodTemplate.CreatedAt = DateTime.UtcNow;
            _context.MethodTemplates.Add(methodTemplate);
            await _context.SaveChangesAsync();
            return methodTemplate;
        }

        public async Task<MethodTemplate> GetByIdAsync(int id, int userId)
        {
            return await GetMethodTemplatesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(mt => mt.Id == id);
        }

        public async Task<List<MethodTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await GetMethodTemplatesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MethodTemplate> UpdateAsync(MethodTemplate methodTemplate)
        {
            _context.MethodTemplates.Update(methodTemplate);
            await _context.SaveChangesAsync();
            return methodTemplate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var methodTemplate = await _context.MethodTemplates
                .FirstOrDefaultAsync(mt => mt.Id == id);

            if (methodTemplate == null) return false;

            _context.MethodTemplates.Remove(methodTemplate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
