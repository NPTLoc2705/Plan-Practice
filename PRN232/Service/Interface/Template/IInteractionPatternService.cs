using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IInteractionPatternService
    {
        Task<InteractionPatternResponse> CreateAsync(InteractionPatternRequest request, int currentUserId);
        Task<InteractionPatternResponse> UpdateAsync(int interactionPatternId, InteractionPatternRequest request, int currentUserId);
        Task<InteractionPatternResponse> GetByIdAsync(int id, int userId);
        Task<List<InteractionPatternResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
