using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface ISkillTemplateService
    {
        Task<SkillTemplateResponse> CreateAsync(SkillTemplateRequest request, int currentUserId);
        Task<SkillTemplateResponse> UpdateAsync(int skillTemplateId, SkillTemplateRequest request, int currentUserId);
        Task<SkillTemplateResponse> GetByIdAsync(int id, int userId);
        Task<List<SkillTemplateResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
