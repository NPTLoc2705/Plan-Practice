using BusinessObject.Lesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IClassRepository
    {
        Task<Class> CreateAsync(Class newClass);
        Task<Class> GetByIdAsync(int id, int userId);
        Task<List<Class>> GetAllByUserIdAsync(int userId);
        Task<Class> UpdateAsync(Class aClass);
        Task<bool> DeleteAsync(int id);
        Task<List<Class>> GetAllByGradeLevelIdAsync(int gradeLevelId, int userId);
    }
}
