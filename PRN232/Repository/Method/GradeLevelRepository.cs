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
    public class GradeLevelRepository : IGradeLevelRepository
    {
        private readonly GradeLevelDAO _gradeLevelDAO;
        public GradeLevelRepository(GradeLevelDAO gradeLevelDAO)
        {
            _gradeLevelDAO = gradeLevelDAO;
        }
        public async Task<GradeLevel> CreateAsync(GradeLevel gradeLevel)
        {
            return await _gradeLevelDAO.CreateAsync(gradeLevel);
        }
        public async Task<GradeLevel> GetByIdAsync(int id, int userId)
        {
            return await _gradeLevelDAO.GetByIdAsync(id, userId);
        }
        public async Task<List<GradeLevel>> GetAllByUserIdAsync(int userId)
        {
            return await _gradeLevelDAO.GetAllByUserIdAsync(userId);
        }
        public async Task<GradeLevel> UpdateAsync(GradeLevel gradeLevel)
        {
            var existing = await _gradeLevelDAO.GetByIdAsync(gradeLevel.Id, gradeLevel.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"GradeLevel with ID {gradeLevel.Id} not found.");
            }
            return await _gradeLevelDAO.UpdateAsync(gradeLevel);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _gradeLevelDAO.DeleteAsync(id);
        }
    }
}
