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
        Task<Class> GetByIdAsync(int id);
        Task<List<Class>> GetAllAsync();
        Task<Class> UpdateAsync(Class aClass);
        Task<bool> DeleteAsync(int id);
    }
}
