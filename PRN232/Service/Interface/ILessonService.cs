using BusinessObject.Dtos.LessonDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ILessonService
    {
        Task<LessonResponse> CreateLessonAsync(LessonRequest lesson, int currentUserId);
        Task<LessonResponse> UpdateLessonAsync(LessonRequest lesson, int currentUserId);
        Task<List<LessonResponse>> GetLessonsByUserIdAsync(int userId);
        Task<bool> DeleteLessonAsync(int id, int currentUserId);
        Task<List<LessonResponse>> GetAllLessonsAsync();
    }
}