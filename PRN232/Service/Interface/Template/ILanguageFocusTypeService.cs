using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface ILanguageFocusTypeService
    {
        Task<LanguageFocusTypeResponse> CreateAsync(LanguageFocusTypeRequest request, int currentUserId);
        Task<LanguageFocusTypeResponse> UpdateAsync(int languageFocusTypeId, LanguageFocusTypeRequest request, int currentUserId);
        Task<LanguageFocusTypeResponse> GetByIdAsync(int id, int userId);
        Task<List<LanguageFocusTypeResponse>> GetAllByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id, int currentUserId);
    }
}
