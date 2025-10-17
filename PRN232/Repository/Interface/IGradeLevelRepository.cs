using BusinessObject.Lesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IGradeLevelRepository
    {
        Task<GradeLevel> CreateAsync(GradeLevel gradeLevel);
        Task<GradeLevel> GetByIdAsync(int id, int userId);
        Task<List<GradeLevel>> GetAllByUserIdAsync(int userId);
        Task<GradeLevel> UpdateAsync(GradeLevel gradeLevel);
        Task<bool> DeleteAsync(int id);
    }
}
