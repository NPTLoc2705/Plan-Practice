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
                UserId = lesson.UserId,

            };
        }

        private Lesson MapToEntity(LessonRequest lesson)
        {
            return new Lesson
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                UserId = lesson.UserId,
            };
        }

        public async Task<LessonResponse> CreateLessonAsync(LessonRequest lesson, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(lesson.Title))
            {
                throw new ArgumentException("Lesson title cannot be empty.");
            }

            if (lesson.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only create lessons for yourself.");
            }

            var lessonEntity = MapToEntity(lesson);
            var createdLesson = await _lessonRepo.CreateLessonAsync(lessonEntity);
            return MapToResponse(createdLesson);
        }

        public async Task<LessonResponse> UpdateLessonAsync(LessonRequest lesson, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(lesson.Title))
            {
                throw new ArgumentException("Lesson title cannot be empty.");
            }

            var existingLesson = await _lessonRepo.GetLessonByIdAsync(lesson.Id);
            if (existingLesson == null)
            {
                throw new KeyNotFoundException($"Lesson with ID {lesson.Id} not found.");
            }

            if (existingLesson.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own lessons.");
            }

            var lessonEntity = MapToEntity(lesson);
            var updatedLesson = await _lessonRepo.UpdateLessonAsync(lessonEntity);
            return MapToResponse(updatedLesson);
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