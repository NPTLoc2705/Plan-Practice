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
    public class AttitudeTemplateRepository : IAttitudeTemplateRepository
    {
        private readonly AttitudeTemplateDAO _attitudeTemplateDAO;

        public AttitudeTemplateRepository(AttitudeTemplateDAO attitudeTemplateDAO)
        {
            _attitudeTemplateDAO = attitudeTemplateDAO;
        }

        public async Task<AttitudeTemplate> CreateAsync(AttitudeTemplate attitudeTemplate)
        {
            return await _attitudeTemplateDAO.CreateAsync(attitudeTemplate);
        }

        public async Task<AttitudeTemplate> GetByIdAsync(int id, int userId)
        {
            return await _attitudeTemplateDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<AttitudeTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await _attitudeTemplateDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<AttitudeTemplate> UpdateAsync(AttitudeTemplate attitudeTemplate)
        {
            var existing = await _attitudeTemplateDAO.GetByIdAsync(attitudeTemplate.Id, attitudeTemplate.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"AttitudeTemplate with ID {attitudeTemplate.Id} not found.");
            }
            return await _attitudeTemplateDAO.UpdateAsync(attitudeTemplate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _attitudeTemplateDAO.DeleteAsync(id);
        }
    }
}
