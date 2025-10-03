using BusinessObject.Lesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ILessonRepository
    {
        Task<Lesson> CreateLessonAsync(Lesson lesson);
        Task<Lesson> GetLessonByIdAsync(int id);
        Task<List<Lesson>> GetAllLessonsAsync();
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(int id);
    }
}
