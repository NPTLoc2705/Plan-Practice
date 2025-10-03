using BusinessObject.Lesson;
using DAL.LessonDAO;
using DAL;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class LessonRepository : ILessonRepository
    {
        private readonly LessonDAO _lessonDAO;

        public LessonRepository(LessonDAO context)
        {
            _lessonDAO = context;
        }

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            if (string.IsNullOrEmpty(lesson.Tite) || string.IsNullOrEmpty(lesson.Content))
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

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            if (string.IsNullOrEmpty(lesson.Tite) || string.IsNullOrEmpty(lesson.Content))
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

        public async Task DeleteLessonAsync(int id)
        {
            var lesson = await _lessonDAO.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                throw new KeyNotFoundException($"Lesson with ID {id} not found.");
            }
            await _lessonDAO.DeleteLessonAsync(id);
        }
    }
}
