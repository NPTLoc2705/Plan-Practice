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

        public async Task<GradeLevel> CreateAsync(GradeLevel gradeLevel)
        {
            _context.GradeLevels.Add(gradeLevel);
            await _context.SaveChangesAsync();
            return gradeLevel;
        }

        public async Task<GradeLevel> GetByIdAsync(int id)
        {
            return await _context.GradeLevels.FindAsync(id);
        }

        public async Task<List<GradeLevel>> GetAllAsync()
        {
            return await _context.GradeLevels.ToListAsync();
        }

        public async Task<GradeLevel> UpdateAsync(GradeLevel gradeLevel)
        {
            _context.Entry(gradeLevel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return gradeLevel;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gradeLevel = await _context.GradeLevels.FindAsync(id);
            if (gradeLevel == null) return false;

            _context.GradeLevels.Remove(gradeLevel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
