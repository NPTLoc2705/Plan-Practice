using BusinessObject.Dtos.LessonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ILessonRepo
    {
        Task<LessonResponse> CreateLessonAsync(LessonRequest lesson);
        Task<LessonResponse> UpdateLessonAsync(LessonRequest lesson);
        Task<List<LessonResponse>> GetLessonsByUserIdAsync(int userId);
        Task<bool> DeleteLessonAsync(int id);
        Task<List<LessonResponse>> GetAllLessonsAsync();
    }
}
