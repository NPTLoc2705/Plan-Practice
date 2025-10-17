using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface ISkillTypeRepository
    {
        Task<SkillType> CreateSkillTypeAsync(SkillType skillType);
        Task<SkillType> GetSkillTypeByIdAsync(int id);
        Task<List<SkillType>> GetAllSkillTypesAsync();
        Task<List<SkillType>> GetSkillTypesByUserIdAsync(int userId);
        Task<SkillType> UpdateSkillTypeAsync(SkillType skillType);
        Task<bool> DeleteSkillTypeAsync(int id);
    }
}
