using BusinessObject.Dtos.LessonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IClassService
    {
        Task<ClassResponse> CreateAsync(ClassRequest request, int userId);
        Task<ClassResponse> UpdateAsync(int id, ClassRequest request, int userId);
        Task<ClassResponse> GetByIdAsync(int id, int userId);
        Task<List<ClassResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<List<ClassResponse>> GetAllByGradeLevelIdAsync(int gradeLevelId, int userId);
    }
}
