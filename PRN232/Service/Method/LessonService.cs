using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.Dtos.LessonDTO;
using Repository.Interface;
using Service.Interface;

namespace Services
{

    public class LessonService : ILessonService
    {
        private readonly ILessonRepo _lessonRepo;

        public LessonService(ILessonRepo lessonRepo)
        {
            _lessonRepo = lessonRepo;
        }

        public async Task<LessonResponse> CreateLessonAsync(LessonRequest lesson, int currentUserId)
        {
            // Validation: Ensure title is not empty
            if (string.IsNullOrWhiteSpace(lesson.Title))
            {
                throw new ArgumentException("Lesson title cannot be empty.");
            }

            // Permission: User must be the lesson owner
            if (lesson.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only create lessons for yourself.");
            }

            return await _lessonRepo.CreateLessonAsync(lesson);
        }

        public async Task<LessonResponse> UpdateLessonAsync(LessonRequest lesson, int currentUserId)
        {
            // Validation: Ensure title is not empty
            if (string.IsNullOrWhiteSpace(lesson.Title))
            {
                throw new ArgumentException("Lesson title cannot be empty.");
            }

            // Fetch lesson to check ownership
            var existingLessons = await _lessonRepo.GetLessonsByUserIdAsync(lesson.UserId);
            var lessonToUpdate = existingLessons.Find(l => l.Id == lesson.Id);
            if (lessonToUpdate == null)
            {
                throw new Exception($"Lesson with ID {lesson.Id} not found.");
            }

            // Permission: User must be the lesson owner (admin check handled by controller)
            if (lesson.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own lessons.");
            }

            return await _lessonRepo.UpdateLessonAsync(lesson);
        }

        public async Task<List<LessonResponse>> GetLessonsByUserIdAsync(int userId)
        {


            return await _lessonRepo.GetLessonsByUserIdAsync(userId);
        }

        public async Task<bool> DeleteLessonAsync(int id, int currentUserId)
        {
            // Fetch lesson to check ownership
            var lessons = await _lessonRepo.GetAllLessonsAsync();
            var lessonToDelete = lessons.Find(l => l.Id == id);
            if (lessonToDelete == null)
            {
                return false;
            }

            // Permission: User must be the lesson owner (admin check handled by controller)
            if (lessonToDelete.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own lessons.");
            }

            return await _lessonRepo.DeleteLessonAsync(id);
        }

        public async Task<List<LessonResponse>> GetAllLessonsAsync()
        {
   
            return await _lessonRepo.GetAllLessonsAsync();
        }


    }
}