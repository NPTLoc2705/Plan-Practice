using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class LanguageFocusTypeDAO
    {
        private readonly PlantPraticeDbContext _context;

        public LanguageFocusTypeDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<LanguageFocusType> GetLanguageFocusTypesWithDetails(int userId)
        {
            return _context.LanguageFocusTypes
                .Where(lft => lft.UserId == userId)
                .Include(lft => lft.User)
                .OrderBy(lft => lft.DisplayOrder);
        }

        public async Task<LanguageFocusType> CreateAsync(LanguageFocusType languageFocusType)
        {
            languageFocusType.CreatedAt = DateTime.UtcNow;
            _context.LanguageFocusTypes.Add(languageFocusType);
            await _context.SaveChangesAsync();
            return languageFocusType;
        }

        public async Task<LanguageFocusType> GetByIdAsync(int id, int userId)
        {
            return await GetLanguageFocusTypesWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(lft => lft.Id == id);
        }

        public async Task<List<LanguageFocusType>> GetAllByUserIdAsync(int userId)
        {
            return await GetLanguageFocusTypesWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LanguageFocusType> UpdateAsync(LanguageFocusType languageFocusType)
        {
            _context.LanguageFocusTypes.Update(languageFocusType);
            await _context.SaveChangesAsync();
            return languageFocusType;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var languageFocusType = await _context.LanguageFocusTypes
                .FirstOrDefaultAsync(lft => lft.Id == id);

            if (languageFocusType == null) return false;

            _context.LanguageFocusTypes.Remove(languageFocusType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
