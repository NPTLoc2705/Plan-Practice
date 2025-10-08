using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepo;

        public LessonService(ILessonRepository lessonRepo)
        {
            _lessonRepo = lessonRepo;
        }

        private LessonResponse MapToResponse(Lesson lesson)
        {
            return new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                GradeLevel = int.TryParse(lesson.GradeLevel, out var grade) ? grade : 0,
                Description = lesson.Description,
                UserId = lesson.UserId
            };
        }

        private Lesson MapToEntity(LessonRequest request, int userId, int? existingId = null)
        {
            return new Lesson
            {
                Id = existingId ?? 0, // 0 for new entities
                Title = request.Title?.Trim(),
                Content = request.Content?.Trim(),
                GradeLevel = request.GradeLevel.ToString(),
                Description = request.Description?.Trim(),
                UserId = userId
            };
        }

        public async Task<LessonResponse> CreateLessonAsync(LessonRequest request, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Lesson title cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new ArgumentException("Lesson content cannot be empty.");
            }

            if (request.GradeLevel < 1 || request.GradeLevel > 12)
            {
                throw new ArgumentException("Grade level must be between 1 and 12.");
            }

            var lessonEntity = MapToEntity(request, currentUserId);
            var createdLesson = await _lessonRepo.CreateLessonAsync(lessonEntity);
            return MapToResponse(createdLesson);
        }

        public async Task<LessonResponse> UpdateLessonAsync(int lessonId, LessonRequest request, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Lesson title cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new ArgumentException("Lesson content cannot be empty.");
            }

            var existingLesson = await _lessonRepo.GetLessonByIdAsync(lessonId);
            if (existingLesson == null)
            {
                throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
            }

            if (existingLesson.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own lessons.");
            }

            var lessonEntity = MapToEntity(request, currentUserId, lessonId);
            var updatedLesson = await _lessonRepo.UpdateLessonAsync(lessonEntity);
            return MapToResponse(updatedLesson);
        }

        public async Task<LessonResponse> GetLessonByIdAsync(int id, int currentUserId)
        {
            var lesson = await _lessonRepo.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                throw new KeyNotFoundException($"Lesson with ID {id} not found.");
            }

            // Optional: Restrict access to own lessons only
            // if (lesson.UserId != currentUserId)
            // {
            //     throw new UnauthorizedAccessException("You can only view your own lessons.");
            // }

            return MapToResponse(lesson);
        }

        public async Task<List<LessonResponse>> GetLessonsByUserIdAsync(int userId)
        {
            var lessons = await _lessonRepo.GetLessonsByUserIdAsync(userId);
            return lessons.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteLessonAsync(int id, int currentUserId)
        {
            var lesson = await _lessonRepo.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                return false;
            }

            if (lesson.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own lessons.");
            }

            return await _lessonRepo.DeleteLessonAsync(id);
        }

        public async Task<List<LessonResponse>> GetAllLessonsAsync()
        {
            var lessons = await _lessonRepo.GetAllLessonsAsync();
            return lessons.Select(MapToResponse).ToList();
        }
    }
}