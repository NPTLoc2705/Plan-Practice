using BusinessObject.Lesson;
using DAL.LessonDAO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class LessonRepository : ILessonRepository
    {
        private readonly LessonDAO _lessonDAO;

        public LessonRepository(LessonDAO lessonDAO)
        {
            _lessonDAO = lessonDAO;
        }

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            if (string.IsNullOrEmpty(lesson.Title) || string.IsNullOrEmpty(lesson.Content))
            {
                throw new ArgumentException("Title and Content are required.");
            }
            return await _lessonDAO.CreateLessonAsync(lesson);
        }

        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            var lesson = await _lessonDAO.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                throw new KeyNotFoundException($"Lesson with ID {id} not found.");
            }
            return lesson;
        }

        public async Task<List<Lesson>> GetAllLessonsAsync()
        {
            return await _lessonDAO.GetAllLessonsAsync();
        }

        public async Task<List<Lesson>> GetLessonsByUserIdAsync(int userId)
        {
            return await _lessonDAO.GetLessonsByUserIdAsync(userId);
        }

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            if (string.IsNullOrEmpty(lesson.Title) || string.IsNullOrEmpty(lesson.Content))
            {
                throw new ArgumentException("Title and Content are required.");
            }
            var existingLesson = await _lessonDAO.GetLessonByIdAsync(lesson.Id);
            if (existingLesson == null)
            {
                throw new KeyNotFoundException($"Lesson with ID {lesson.Id} not found.");
            }
            return await _lessonDAO.UpdateLessonAsync(lesson);
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            var lesson = await _lessonDAO.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                return false;
            }
            return await _lessonDAO.DeleteLessonAsync(id);
        }
    }
}