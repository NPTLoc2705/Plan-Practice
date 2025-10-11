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
    public class ClassRepository : IClassRepository
    {
        private readonly ClassDAO _classDAO;

        public ClassRepository(ClassDAO classDAO)
        {
            _classDAO = classDAO;
        }

        public async Task<Class> CreateAsync(Class newClass) => await _classDAO.CreateAsync(newClass);
        public async Task<Class> GetByIdAsync(int id) => await _classDAO.GetByIdAsync(id);
        public async Task<List<Class>> GetAllAsync() => await _classDAO.GetAllAsync();
        public async Task<Class> UpdateAsync(Class aClass) => await _classDAO.UpdateAsync(aClass);
        public async Task<bool> DeleteAsync(int id) => await _classDAO.DeleteAsync(id);
    }
}
