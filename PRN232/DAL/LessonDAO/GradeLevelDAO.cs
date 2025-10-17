using BusinessObject.Lesson;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO
{
    public class GradeLevelDAO
    {
        private readonly PlantPraticeDbContext _context;
        public GradeLevelDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }
        private IQueryable<GradeLevel> GetGradeLevelsWithDetails(int userId)
        {
            return _context.GradeLevels
            .Where(gl => gl.UserId == userId)
            .Include(gl => gl.User);
        }
        public async Task<GradeLevel> CreateAsync(GradeLevel gradeLevel)
        {
            _context.GradeLevels.Add(gradeLevel);
            await _context.SaveChangesAsync();
            return gradeLevel;
        }
        public async Task<GradeLevel> GetByIdAsync(int id, int userId)
        {
            return await GetGradeLevelsWithDetails(userId)
            .AsNoTracking()
            .FirstOrDefaultAsync(gl => gl.Id == id);
        }
        public async Task<List<GradeLevel>> GetAllByUserIdAsync(int userId)
        {
            return await GetGradeLevelsWithDetails(userId)
            .AsNoTracking()
            .OrderBy(gl => gl.Level)
            .ToListAsync();
        }
        public async Task<GradeLevel> UpdateAsync(GradeLevel gradeLevel)
        {
            _context.GradeLevels.Update(gradeLevel);
            await _context.SaveChangesAsync();
            return gradeLevel;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var gradeLevel = await _context.GradeLevels
            .FirstOrDefaultAsync(gl => gl.Id == id);
            if (gradeLevel == null) return false;
            _context.GradeLevels.Remove(gradeLevel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
