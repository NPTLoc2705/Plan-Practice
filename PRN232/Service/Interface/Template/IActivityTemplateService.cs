using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IActivityTemplateService
    {
        Task<ActivityTemplateResponse> CreateAsync(ActivityTemplateRequest request, int currentUserId);
        Task<ActivityTemplateResponse> UpdateAsync(int activityTemplateId, ActivityTemplateRequest request, int currentUserId);
        Task<ActivityTemplateResponse> GetByIdAsync(int id, int userId);
        Task<List<ActivityTemplateResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
