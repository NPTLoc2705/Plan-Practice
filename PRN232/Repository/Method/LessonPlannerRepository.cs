using BusinessObject.Lesson;
using DAL.LessonDAO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class LessonPlannerRepository : ILessonPlannerRepository
    {
        private readonly LessonPlannerDAO _lessonPlannerDAO;

        public LessonPlannerRepository(LessonPlannerDAO lessonPlannerDAO)
        {
            _lessonPlannerDAO = lessonPlannerDAO;
        }

        public async Task<LessonPlanner> CreateLessonPlannerAsync(LessonPlanner lessonPlanner)
        {
            return await _lessonPlannerDAO.CreateLessonPlannerAsync(lessonPlanner);
        }

        public async Task<LessonPlanner> GetLessonPlannerByIdAsync(int id)
        {
            return await _lessonPlannerDAO.GetLessonPlannerByIdAsync(id);
        }

        public async Task<List<LessonPlanner>> GetAllLessonPlannersAsync()
        {
            return await _lessonPlannerDAO.GetAllLessonPlannersAsync();
        }

        public async Task<List<LessonPlanner>> GetLessonPlannersByUserIdAsync(int userId)
        {
            return await _lessonPlannerDAO.GetLessonPlannersByUserIdAsync(userId);
        }

        public async Task<LessonPlanner> UpdateLessonPlannerAsync(LessonPlanner lessonPlanner)
        {
            var existing = await _lessonPlannerDAO.GetLessonPlannerByIdAsync(lessonPlanner.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"LessonPlanner with ID {lessonPlanner.Id} not found.");
            }
            return await _lessonPlannerDAO.UpdateLessonPlannerAsync(lessonPlanner);
        }

        public async Task<bool> DeleteLessonPlannerAsync(int id)
        {
            var existing = await _lessonPlannerDAO.GetLessonPlannerByIdAsync(id);
            if (existing == null)
            {
                return false;
            }
            return await _lessonPlannerDAO.DeleteLessonPlannerAsync(id);
        }
    }
}