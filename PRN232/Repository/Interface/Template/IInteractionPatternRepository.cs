using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
    public interface IInteractionPatternRepository
    {
        Task<InteractionPattern> CreateAsync(InteractionPattern interactionPattern);
        Task<InteractionPattern> GetByIdAsync(int id, int userId);
        Task<List<InteractionPattern>> GetAllByUserIdAsync(int userId);
        Task<InteractionPattern> UpdateAsync(InteractionPattern interactionPattern);
        Task<bool> DeleteAsync(int id);
    }
}
