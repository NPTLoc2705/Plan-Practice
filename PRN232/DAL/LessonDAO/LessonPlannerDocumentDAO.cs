using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject;
using BusinessObject.Lesson;
using Microsoft.EntityFrameworkCore;

namespace DAL.LessonDAO
{
    public class LessonPlannerDocumentDAO
    {
        private readonly PlantPraticeDbContext _context;

        public LessonPlannerDocumentDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        public async Task<LessonPlannerDocument> CreateDocumentAsync(LessonPlannerDocument document)
        {
            document.CreatedAt = DateTime.UtcNow;
            _context.LessonPlannerDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<LessonPlannerDocument> GetDocumentByIdAsync(int id)
        {
            return await _context.LessonPlannerDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<LessonPlannerDocument>> GetDocumentsByLessonPlannerIdAsync(int lessonPlannerId)
        {
            return await _context.LessonPlannerDocuments
                .Where(d => d.LessonPlannerId == lessonPlannerId)
                .OrderByDescending(d => d.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var document = await _context.LessonPlannerDocuments.FindAsync(id);
            if (document == null) return false;

            _context.LessonPlannerDocuments.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
