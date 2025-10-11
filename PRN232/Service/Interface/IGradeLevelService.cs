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
        Task<GradeLevelResponse> CreateAsync(GradeLevelRequest request);
        Task<GradeLevelResponse> GetByIdAsync(int id);
        Task<List<GradeLevelResponse>> GetAllAsync();
        Task<GradeLevelResponse> UpdateAsync(int id, GradeLevelRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
