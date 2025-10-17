using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IObjectiveTemplateService
    {
        Task<ObjectiveTemplateResponse> CreateAsync(ObjectiveTemplateRequest request, int currentUserId);
        Task<ObjectiveTemplateResponse> UpdateAsync(int objectiveTemplateId, ObjectiveTemplateRequest request, int currentUserId);
        Task<ObjectiveTemplateResponse> GetByIdAsync(int id, int userId);
        Task<List<ObjectiveTemplateResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
