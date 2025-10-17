using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface IMethodTemplateRepository
    {
        Task<MethodTemplate> CreateAsync(MethodTemplate methodTemplate);
        Task<MethodTemplate> GetByIdAsync(int id, int userId);
        Task<List<MethodTemplate>> GetAllByUserIdAsync(int userId);
        Task<MethodTemplate> UpdateAsync(MethodTemplate methodTemplate);
        Task<bool> DeleteAsync(int id);
    }
}
