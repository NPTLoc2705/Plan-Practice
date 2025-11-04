using BusinessObject.Lesson;
using DAL.LessonDAO;
using Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class LessonPlannerDocumentRepository : ILessonPlannerDocumentRepository
    {
        private readonly LessonPlannerDocumentDAO _documentDAO;

        public LessonPlannerDocumentRepository(LessonPlannerDocumentDAO documentDAO)
        {
            _documentDAO = documentDAO;
        }

        public async Task<LessonPlannerDocument> CreateDocumentAsync(LessonPlannerDocument document)
        {
            return await _documentDAO.CreateDocumentAsync(document);
        }

        public async Task<LessonPlannerDocument> GetDocumentByIdAsync(int id)
        {
            return await _documentDAO.GetDocumentByIdAsync(id);
        }

        public async Task<List<LessonPlannerDocument>> GetDocumentsByLessonPlannerIdAsync(int lessonPlannerId)
        {
            return await _documentDAO.GetDocumentsByLessonPlannerIdAsync(lessonPlannerId);
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var existing = await _documentDAO.GetDocumentByIdAsync(id);
            if (existing == null)
            {
                return false;
            }
            return await _documentDAO.DeleteDocumentAsync(id);
        }
    }
}
