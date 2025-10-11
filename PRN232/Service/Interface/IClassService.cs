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
        Task<ClassResponse> CreateAsync(ClassRequest request);
        Task<ClassResponse> GetByIdAsync(int id);
        Task<List<ClassResponse>> GetAllAsync();
        Task<ClassResponse> UpdateAsync(int id, ClassRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
