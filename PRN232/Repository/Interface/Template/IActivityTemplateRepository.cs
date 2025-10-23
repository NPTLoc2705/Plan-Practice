using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface IActivityTemplateRepository
    {
        Task<ActivityTemplate> CreateAsync(ActivityTemplate activityTemplate);
        Task<ActivityTemplate> GetByIdAsync(int id, int userId);
        Task<List<ActivityTemplate>> GetAllByUserIdAsync(int userId);
        Task<ActivityTemplate> UpdateAsync(ActivityTemplate activityTemplate);
        Task<bool> DeleteAsync(int id);
    }
}
