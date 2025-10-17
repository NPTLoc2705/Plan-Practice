using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IAttitudeTemplateService
    {
        Task<AttitudeTemplateResponse> CreateAsync(AttitudeTemplateRequest request, int currentUserId);
        Task<AttitudeTemplateResponse> UpdateAsync(int attitudeTemplateId, AttitudeTemplateRequest request, int currentUserId);
        Task<AttitudeTemplateResponse> GetByIdAsync(int id, int userId);
        Task<List<AttitudeTemplateResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
