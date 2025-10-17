using BusinessObject.Lesson.Template;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LessonDAO.Template
{
    public class InteractionPatternDAO
    {
        private readonly PlantPraticeDbContext _context;

        public InteractionPatternDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        private IQueryable<InteractionPattern> GetInteractionPatternsWithDetails(int userId)
        {
            return _context.InteractionPatterns
                .Where(ip => ip.UserId == userId)
                .Include(ip => ip.User)
                .OrderBy(ip => ip.Name);
        }

        public async Task<InteractionPattern> CreateAsync(InteractionPattern interactionPattern)
        {
            interactionPattern.CreatedAt = DateTime.UtcNow;
            _context.InteractionPatterns.Add(interactionPattern);
            await _context.SaveChangesAsync();
            return interactionPattern;
        }

        public async Task<InteractionPattern> GetByIdAsync(int id, int userId)
        {
            return await GetInteractionPatternsWithDetails(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(ip => ip.Id == id);
        }

        public async Task<List<InteractionPattern>> GetAllByUserIdAsync(int userId)
        {
            return await GetInteractionPatternsWithDetails(userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<InteractionPattern> UpdateAsync(InteractionPattern interactionPattern)
        {
            _context.InteractionPatterns.Update(interactionPattern);
            await _context.SaveChangesAsync();
            return interactionPattern;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var interactionPattern = await _context.InteractionPatterns
                .FirstOrDefaultAsync(ip => ip.Id == id);

            if (interactionPattern == null) return false;

            _context.InteractionPatterns.Remove(interactionPattern);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
