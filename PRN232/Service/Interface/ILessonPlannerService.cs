using BusinessObject.Dtos.LessonDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ILessonPlannerService
    {
        Task<LessonPlannerResponse> CreateLessonPlannerAsync(LessonPlannerRequest request, int currentUserId);
        Task<LessonPlannerResponse> UpdateLessonPlannerAsync(int lessonPlannerId, LessonPlannerRequest request, int currentUserId);
        Task<LessonPlannerResponse> GetLessonPlannerByIdAsync(int id);
        Task<List<LessonPlannerResponse>> GetLessonPlannersByUserIdAsync(int userId);
        Task<List<LessonPlannerResponse>> GetAllLessonPlannersAsync();
        Task<bool> DeleteLessonPlannerAsync(int id, int currentUserId);
    }
}