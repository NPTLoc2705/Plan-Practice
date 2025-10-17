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
    public class LanguageFocusTypeRepository : ILanguageFocusTypeRepository
    {
        private readonly LanguageFocusTypeDAO _languageFocusTypeDAO;

        public LanguageFocusTypeRepository(LanguageFocusTypeDAO languageFocusTypeDAO)
        {
            _languageFocusTypeDAO = languageFocusTypeDAO;
        }

        public async Task<LanguageFocusType> CreateAsync(LanguageFocusType languageFocusType)
        {
            return await _languageFocusTypeDAO.CreateAsync(languageFocusType);
        }

        public async Task<LanguageFocusType> GetByIdAsync(int id, int userId)
        {
            return await _languageFocusTypeDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<LanguageFocusType>> GetAllByUserIdAsync(int userId)
        {
            return await _languageFocusTypeDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<LanguageFocusType> UpdateAsync(LanguageFocusType languageFocusType)
        {
            var existing = await _languageFocusTypeDAO.GetByIdAsync(languageFocusType.Id, languageFocusType.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"LanguageFocusType with ID {languageFocusType.Id} not found.");
            }
            return await _languageFocusTypeDAO.UpdateAsync(languageFocusType);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _languageFocusTypeDAO.DeleteAsync(id);
        }
    }
}
