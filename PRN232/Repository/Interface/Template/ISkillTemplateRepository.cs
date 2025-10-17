using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface ISkillTemplateRepository
    {
        Task<SkillTemplate> CreateAsync(SkillTemplate skillTemplate);
        Task<SkillTemplate> GetByIdAsync(int id, int userId);
        Task<List<SkillTemplate>> GetAllByUserIdAsync(int userId);
        Task<SkillTemplate> UpdateAsync(SkillTemplate skillTemplate);
        Task<bool> DeleteAsync(int id);
    }
}
