using BusinessObject.Lesson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ILessonRepository
    {
        Task<Lesson> CreateLessonAsync(Lesson lesson);
        Task<Lesson> GetLessonByIdAsync(int id);
        Task<List<Lesson>> GetAllLessonsAsync();
        Task<List<Lesson>> GetLessonsByUserIdAsync(int userId);
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
        Task<bool> DeleteLessonAsync(int id);
    }
}