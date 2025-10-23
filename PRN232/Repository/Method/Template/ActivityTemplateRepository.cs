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
    public class ActivityTemplateRepository : IActivityTemplateRepository
    {
        private readonly ActivityTemplateDAO _activityTemplateDAO;

        public ActivityTemplateRepository(ActivityTemplateDAO activityTemplateDAO)
        {
            _activityTemplateDAO = activityTemplateDAO;
        }

        public async Task<ActivityTemplate> CreateAsync(ActivityTemplate activityTemplate)
        {
            return await _activityTemplateDAO.CreateAsync(activityTemplate);
        }

        public async Task<ActivityTemplate> GetByIdAsync(int id, int userId)
        {
            return await _activityTemplateDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<ActivityTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await _activityTemplateDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<ActivityTemplate> UpdateAsync(ActivityTemplate activityTemplate)
        {
            var existing = await _activityTemplateDAO.GetByIdAsync(activityTemplate.Id, activityTemplate.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"ActivityTemplate with ID {activityTemplate.Id} not found.");
            }
            return await _activityTemplateDAO.UpdateAsync(activityTemplate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _activityTemplateDAO.DeleteAsync(id);
        }
    }
}
