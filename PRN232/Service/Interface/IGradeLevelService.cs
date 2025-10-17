using BusinessObject.Dtos.LessonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IGradeLevelService
    {
        Task<GradeLevelResponse> CreateAsync(GradeLevelRequest request, int currentUserId);
        Task<GradeLevelResponse> UpdateAsync(int gradeLevelId, GradeLevelRequest request, int currentUserId);
        Task<GradeLevelResponse> GetByIdAsync(int id, int userId);
        Task<List<GradeLevelResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
