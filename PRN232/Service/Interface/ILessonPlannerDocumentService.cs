using BusinessObject.Dtos.LessonDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ILessonPlannerDocumentService
    {
        Task<LessonPlannerDocumentResponse> CreateDocumentAsync(LessonPlannerDocumentRequest request);
        Task<LessonPlannerDocumentResponse> GetDocumentByIdAsync(int id);
        Task<List<LessonPlannerDocumentResponse>> GetDocumentsByLessonPlannerIdAsync(int lessonPlannerId);
        Task<bool> DeleteDocumentAsync(int id);
    }
}
