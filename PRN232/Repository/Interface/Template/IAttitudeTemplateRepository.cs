using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface IAttitudeTemplateRepository
    {
        Task<AttitudeTemplate> CreateAsync(AttitudeTemplate attitudeTemplate);
        Task<AttitudeTemplate> GetByIdAsync(int id, int userId);
        Task<List<AttitudeTemplate>> GetAllByUserIdAsync(int userId);
        Task<AttitudeTemplate> UpdateAsync(AttitudeTemplate attitudeTemplate);
        Task<bool> DeleteAsync(int id);
    }
}
