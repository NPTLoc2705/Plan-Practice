using BusinessObject.Lesson;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO
{
    public class ClassDAO
    {
        private readonly PlantPraticeDbContext _context;
        public ClassDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }
        private IQueryable<Class> GetClassesWithDetails(int userId)
        {
            return _context.Classes
            .Where(c => c.GradeLevel.UserId == userId) // UserId comes through GradeLevel
            .Include(c => c.GradeLevel);
        }
        public async Task<Class> CreateAsync(Class newClass)
        {
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();
            return newClass;
        }
        public async Task<Class> GetByIdAsync(int id, int userId)
        {
            return await GetClassesWithDetails(userId)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Class>> GetAllByUserIdAsync(int userId)
        {
            return await GetClassesWithDetails(userId)
            .AsNoTracking()
            .ToListAsync();
        }
        public async Task<Class> UpdateAsync(Class aClass)
        {
            _context.Classes.Update(aClass);
            await _context.SaveChangesAsync();
            return aClass;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var aClass = await _context.Classes
            .FirstOrDefaultAsync(c => c.Id == id);
            if (aClass == null) return false;
            _context.Classes.Remove(aClass);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Class>> GetAllByGradeLevelIdAsync(int gradeLevelId, int userId)
        {
            return await GetClassesWithDetails(userId)
            .Where(c => c.GradeLevelId == gradeLevelId)
            .AsNoTracking()
            .ToListAsync();
        }
    }
}
