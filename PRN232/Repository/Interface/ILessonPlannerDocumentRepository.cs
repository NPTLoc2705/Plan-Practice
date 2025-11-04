using BusinessObject.Lesson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ILessonPlannerDocumentRepository
    {
        Task<LessonPlannerDocument> CreateDocumentAsync(LessonPlannerDocument document);
        Task<LessonPlannerDocument> GetDocumentByIdAsync(int id);
        Task<List<LessonPlannerDocument>> GetDocumentsByLessonPlannerIdAsync(int lessonPlannerId);
        Task<bool> DeleteDocumentAsync(int id);
    }
}
