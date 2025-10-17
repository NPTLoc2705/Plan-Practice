using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IMethodTemplateService
    {
        Task<MethodTemplateResponse> CreateAsync(MethodTemplateRequest request, int currentUserId);
        Task<MethodTemplateResponse> UpdateAsync(int methodTemplateId, MethodTemplateRequest request, int currentUserId);
        Task<MethodTemplateResponse> GetByIdAsync(int id, int userId);
        Task<List<MethodTemplateResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
