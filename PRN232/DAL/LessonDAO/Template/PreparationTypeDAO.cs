using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class PreparationTypeDAO
    {
        private readonly PlantPraticeDbContext _context;

        public PreparationTypeDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<PreparationType> GetPreparationTypesWithDetails(int userId)
        {
            return _context.PreparationTypes
                .Where(pt => pt.UserId == userId)
                .Include(pt => pt.User)
                .OrderBy(pt => pt.Name);
        }

        public async Task<PreparationType> CreateAsync(PreparationType preparationType)
        {
            preparationType.CreatedAt = DateTime.UtcNow;
            _context.PreparationTypes.Add(preparationType);
            await _context.SaveChangesAsync();
            return preparationType;
        }

        public async Task<PreparationType> GetByIdAsync(int id, int userId)
        {
            return await GetPreparationTypesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task<List<PreparationType>> GetAllByUserIdAsync(int userId)
        {
            return await GetPreparationTypesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PreparationType> UpdateAsync(PreparationType preparationType)
        {
            _context.PreparationTypes.Update(preparationType);
            await _context.SaveChangesAsync();
            return preparationType;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var preparationType = await _context.PreparationTypes
                .FirstOrDefaultAsync(pt => pt.Id == id);

            if (preparationType == null) return false;

            _context.PreparationTypes.Remove(preparationType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
