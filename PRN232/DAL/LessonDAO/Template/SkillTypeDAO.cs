using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class SkillTypeDAO
    {
        private readonly PlantPraticeDbContext _context;

        public SkillTypeDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<SkillType> GetSkillTypesWithDetails()
        {
            return _context.SkillTypes
                .Include(st => st.User);
        }

        public async Task<SkillType> CreateSkillTypeAsync(SkillType skillType)
        {
            skillType.CreatedAt = DateTime.UtcNow;
            _context.SkillTypes.Add(skillType);
            await _context.SaveChangesAsync();
            return skillType;
        }

        public async Task<SkillType> GetSkillTypeByIdAsync(int id)
        {
            return await GetSkillTypesWithDetails()
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<List<SkillType>> GetAllSkillTypesAsync()
        {
            return await _context.SkillTypes
                .Include(st => st.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SkillType>> GetSkillTypesByUserIdAsync(int userId)
        {
            return await _context.SkillTypes
                .Where(st => st.UserId == userId)
                .Include(st => st.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SkillType> UpdateSkillTypeAsync(SkillType skillType)
        {
            _context.SkillTypes.Update(skillType);
            await _context.SaveChangesAsync();
            return skillType;
        }

        public async Task<bool> DeleteSkillTypeAsync(int id)
        {
            var skillType = await _context.SkillTypes
                .FirstOrDefaultAsync(st => st.Id == id);

            if (skillType == null) return false;

            _context.SkillTypes.Remove(skillType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
