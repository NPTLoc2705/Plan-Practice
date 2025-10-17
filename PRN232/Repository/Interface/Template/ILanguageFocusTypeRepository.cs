using BusinessObject.Lesson.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Template
{
public interface ILanguageFocusTypeRepository
    {
        Task<LanguageFocusType> CreateAsync(LanguageFocusType languageFocusType);
        Task<LanguageFocusType> GetByIdAsync(int id, int userId);
        Task<List<LanguageFocusType>> GetAllByUserIdAsync(int userId);
        Task<LanguageFocusType> UpdateAsync(LanguageFocusType languageFocusType);
        Task<bool> DeleteAsync(int id);
    }
}
