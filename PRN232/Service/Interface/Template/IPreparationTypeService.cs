using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IPreparationTypeService
    {
        Task<PreparationTypeResponse> CreateAsync(PreparationTypeRequest request, int currentUserId);
        Task<PreparationTypeResponse> UpdateAsync(int preparationTypeId, PreparationTypeRequest request, int currentUserId);
        Task<PreparationTypeResponse> GetByIdAsync(int id, int userId);
        Task<List<PreparationTypeResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
