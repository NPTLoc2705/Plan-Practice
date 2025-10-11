using BusinessObject.Lesson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ILessonPlannerRepository
    {
        Task<LessonPlanner> CreateLessonPlannerAsync(LessonPlanner lessonPlanner);
        Task<LessonPlanner> GetLessonPlannerByIdAsync(int id);
        Task<List<LessonPlanner>> GetAllLessonPlannersAsync();
        Task<List<LessonPlanner>> GetLessonPlannersByUserIdAsync(int userId);
        Task<LessonPlanner> UpdateLessonPlannerAsync(LessonPlanner lessonPlanner);
        Task<bool> DeleteLessonPlannerAsync(int id);
    }
}