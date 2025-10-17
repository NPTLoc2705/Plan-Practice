using BusinessObject.Lesson;
using DAL.LessonDAO;
using DAL.LessonDAO.Template;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class ClassRepository : IClassRepository
    {
        private readonly ClassDAO _classDAO;
        public ClassRepository(ClassDAO classDAO)
        {
            _classDAO = classDAO;
        }
        public async Task<Class> CreateAsync(Class newClass)
        {
            return await _classDAO.CreateAsync(newClass);
        }
        public async Task<Class> GetByIdAsync(int id, int userId)
        {
            return await _classDAO.GetByIdAsync(id, userId);
        }
        public async Task<List<Class>> GetAllByUserIdAsync(int userId)
        {
            return await _classDAO.GetAllByUserIdAsync(userId);
        }
        public async Task<Class> UpdateAsync(Class aClass)
        {
            var existing = await _classDAO.GetByIdAsync(aClass.Id, aClass.GradeLevel.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Class with ID {aClass.Id} not found.");
            }
            return await _classDAO.UpdateAsync(aClass);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _classDAO.DeleteAsync(id);
        }
        public async Task<List<Class>> GetAllByGradeLevelIdAsync(int gradeLevelId, int userId)
        {
            return await _classDAO.GetAllByGradeLevelIdAsync(gradeLevelId, userId);
        }
    }
}
