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

        private IQueryable<Class> GetClassesWithDetails()
        {
            return _context.Classes.Include(c => c.GradeLevel);
        }

        public async Task<Class> CreateAsync(Class newClass)
        {
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();
            return newClass;
        }

        public async Task<Class> GetByIdAsync(int id)
        {
            return await GetClassesWithDetails().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Class>> GetAllAsync()
        {
            return await GetClassesWithDetails().ToListAsync();
        }

        public async Task<Class> UpdateAsync(Class aClass)
        {
            _context.Entry(aClass).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return aClass;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var aClass = await _context.Classes.FindAsync(id);
            if (aClass == null) return false;

            _context.Classes.Remove(aClass);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
