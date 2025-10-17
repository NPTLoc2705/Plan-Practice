using BusinessObject.Lesson.Template;
using DAL.LessonDAO.Template;
using Repository.Interface.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Template
{
    public class SkillTemplateRepository : ISkillTemplateRepository
    {
        private readonly SkillTemplateDAO _skillTemplateDAO;

        public SkillTemplateRepository(SkillTemplateDAO skillTemplateDAO)
        {
            _skillTemplateDAO = skillTemplateDAO;
        }

        public async Task<SkillTemplate> CreateAsync(SkillTemplate skillTemplate)
        {
            return await _skillTemplateDAO.CreateAsync(skillTemplate);
        }

        public async Task<SkillTemplate> GetByIdAsync(int id, int userId)
        {
            return await _skillTemplateDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<SkillTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await _skillTemplateDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<SkillTemplate> UpdateAsync(SkillTemplate skillTemplate)
        {
            var existing = await _skillTemplateDAO.GetByIdAsync(skillTemplate.Id, skillTemplate.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"SkillTemplate with ID {skillTemplate.Id} not found.");
            }
            return await _skillTemplateDAO.UpdateAsync(skillTemplate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _skillTemplateDAO.DeleteAsync(id);
        }
    }
}
