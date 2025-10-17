using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface IObjectiveTemplateRepository
    {
        Task<ObjectiveTemplate> CreateAsync(ObjectiveTemplate objectiveTemplate);
        Task<ObjectiveTemplate> GetByIdAsync(int id, int userId);
        Task<List<ObjectiveTemplate>> GetAllByUserIdAsync(int userId);
        Task<ObjectiveTemplate> UpdateAsync(ObjectiveTemplate objectiveTemplate);
        Task<bool> DeleteAsync(int id);
    }
}
