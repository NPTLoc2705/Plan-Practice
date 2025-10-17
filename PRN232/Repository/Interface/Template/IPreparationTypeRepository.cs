using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface IPreparationTypeRepository
    {
        Task<PreparationType> CreateAsync(PreparationType preparationType);
        Task<PreparationType> GetByIdAsync(int id, int userId);
        Task<List<PreparationType>> GetAllByUserIdAsync(int userId);
        Task<PreparationType> UpdateAsync(PreparationType preparationType);
        Task<bool> DeleteAsync(int id);
    }
}
