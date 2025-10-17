using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface ISkillTypeService
    {
        Task<SkillTypeResponse> CreateSkillTypeAsync(SkillTypeRequest request, int currentUserId);
        Task<SkillTypeResponse> UpdateSkillTypeAsync(int skillTypeId, SkillTypeRequest request, int currentUserId);
        Task<SkillTypeResponse> GetSkillTypeByIdAsync(int id);
        Task<List<SkillTypeResponse>> GetSkillTypesByUserIdAsync(int userId);
        Task<List<SkillTypeResponse>> GetAllSkillTypesAsync();
        Task<bool> DeleteSkillTypeAsync(int id, int currentUserId);
    }
}
