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
    public class SkillTypeRepository : ISkillTypeRepository
    {
        private readonly SkillTypeDAO _skillTypeDAO;

        public SkillTypeRepository(SkillTypeDAO skillTypeDAO)
        {
            _skillTypeDAO = skillTypeDAO;
        }

        public async Task<SkillType> CreateSkillTypeAsync(SkillType skillType)
        {
            return await _skillTypeDAO.CreateSkillTypeAsync(skillType);
        }

        public async Task<SkillType> GetSkillTypeByIdAsync(int id)
        {
            return await _skillTypeDAO.GetSkillTypeByIdAsync(id);
        }

        public async Task<List<SkillType>> GetAllSkillTypesAsync()
        {
            return await _skillTypeDAO.GetAllSkillTypesAsync();
        }

        public async Task<List<SkillType>> GetSkillTypesByUserIdAsync(int userId)
        {
            return await _skillTypeDAO.GetSkillTypesByUserIdAsync(userId);
        }

        public async Task<SkillType> UpdateSkillTypeAsync(SkillType skillType)
        {
            var existing = await _skillTypeDAO.GetSkillTypeByIdAsync(skillType.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"SkillType with ID {skillType.Id} not found.");
            }
            return await _skillTypeDAO.UpdateSkillTypeAsync(skillType);
        }

        public async Task<bool> DeleteSkillTypeAsync(int id)
        {
            var existing = await _skillTypeDAO.GetSkillTypeByIdAsync(id);
            if (existing == null)
            {
                return false;
            }
            return await _skillTypeDAO.DeleteSkillTypeAsync(id);
        }
    }
}
