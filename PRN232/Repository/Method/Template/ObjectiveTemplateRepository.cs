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
    public class ObjectiveTemplateRepository : IObjectiveTemplateRepository
    {
        private readonly ObjectiveTemplateDAO _objectiveTemplateDAO;

        public ObjectiveTemplateRepository(ObjectiveTemplateDAO objectiveTemplateDAO)
        {
            _objectiveTemplateDAO = objectiveTemplateDAO;
        }

        public async Task<ObjectiveTemplate> CreateAsync(ObjectiveTemplate objectiveTemplate)
        {
            return await _objectiveTemplateDAO.CreateAsync(objectiveTemplate);
        }

        public async Task<ObjectiveTemplate> GetByIdAsync(int id, int userId)
        {
            return await _objectiveTemplateDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<ObjectiveTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await _objectiveTemplateDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<ObjectiveTemplate> UpdateAsync(ObjectiveTemplate objectiveTemplate)
        {
            var existing = await _objectiveTemplateDAO.GetByIdAsync(objectiveTemplate.Id, objectiveTemplate.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"ObjectiveTemplate with ID {objectiveTemplate.Id} not found.");
            }
            return await _objectiveTemplateDAO.UpdateAsync(objectiveTemplate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _objectiveTemplateDAO.DeleteAsync(id);
        }
    }
}
