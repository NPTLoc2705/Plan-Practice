using BusinessObject.Lesson;
using DAL.LessonDAO;
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

        public async Task<GradeLevel> CreateAsync(GradeLevel gradeLevel) => await _gradeLevelDAO.CreateAsync(gradeLevel);
        public async Task<GradeLevel> GetByIdAsync(int id) => await _gradeLevelDAO.GetByIdAsync(id);
        public async Task<List<GradeLevel>> GetAllAsync() => await _gradeLevelDAO.GetAllAsync();
        public async Task<GradeLevel> UpdateAsync(GradeLevel gradeLevel) => await _gradeLevelDAO.UpdateAsync(gradeLevel);
        public async Task<bool> DeleteAsync(int id) => await _gradeLevelDAO.DeleteAsync(id);
    }
}
