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
    public class InteractionPatternRepository : IInteractionPatternRepository
    {
        private readonly InteractionPatternDAO _interactionPatternDAO;

        public InteractionPatternRepository(InteractionPatternDAO interactionPatternDAO)
        {
            _interactionPatternDAO = interactionPatternDAO;
        }

        public async Task<InteractionPattern> CreateAsync(InteractionPattern interactionPattern)
        {
            return await _interactionPatternDAO.CreateAsync(interactionPattern);
        }

        public async Task<InteractionPattern> GetByIdAsync(int id, int userId)
        {
            return await _interactionPatternDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<InteractionPattern>> GetAllByUserIdAsync(int userId)
        {
            return await _interactionPatternDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<InteractionPattern> UpdateAsync(InteractionPattern interactionPattern)
        {
            var existing = await _interactionPatternDAO.GetByIdAsync(interactionPattern.Id, interactionPattern.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"InteractionPattern with ID {interactionPattern.Id} not found.");
            }
            return await _interactionPatternDAO.UpdateAsync(interactionPattern);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _interactionPatternDAO.DeleteAsync(id);
        }
    }
}
