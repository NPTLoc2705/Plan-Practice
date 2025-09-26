using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using DAL;
using DAL.LessonDAO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class LessonRepo : ILessonRepo
    {
        private readonly LessonDAO _lessonDAO;

        public LessonRepo(LessonDAO lessonDAO)
        {
            _lessonDAO = lessonDAO;
        }

        public async Task<LessonResponse> CreateLessonAsync(LessonRequest lesson)
        {
            return await _lessonDAO.CreateLesson(lesson);
        }

        public async Task<LessonResponse> UpdateLessonAsync(LessonRequest lesson)
        {
            return await _lessonDAO.UpdateLesson(lesson);
        }

        public async Task<List<LessonResponse>> GetLessonsByUserIdAsync(int userId)
        {
            return await _lessonDAO.ViewLesson(userId);
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            return await _lessonDAO.DeleteLesson(id);
        }

        public async Task<List<LessonResponse>> GetAllLessonsAsync()
        {
            return await _lessonDAO.ViewAllLesson();
        }
    }
}