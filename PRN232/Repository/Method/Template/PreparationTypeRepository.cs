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
    public class PreparationTypeRepository : IPreparationTypeRepository
    {
        private readonly PreparationTypeDAO _preparationTypeDAO;

        public PreparationTypeRepository(PreparationTypeDAO preparationTypeDAO)
        {
            _preparationTypeDAO = preparationTypeDAO;
        }

        public async Task<PreparationType> CreateAsync(PreparationType preparationType)
        {
            return await _preparationTypeDAO.CreateAsync(preparationType);
        }

        public async Task<PreparationType> GetByIdAsync(int id, int userId)
        {
            return await _preparationTypeDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<PreparationType>> GetAllByUserIdAsync(int userId)
        {
            return await _preparationTypeDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<PreparationType> UpdateAsync(PreparationType preparationType)
        {
            var existing = await _preparationTypeDAO.GetByIdAsync(preparationType.Id, preparationType.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"PreparationType with ID {preparationType.Id} not found.");
            }
            return await _preparationTypeDAO.UpdateAsync(preparationType);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _preparationTypeDAO.DeleteAsync(id);
        }
    }
}
