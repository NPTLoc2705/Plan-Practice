using BusinessObject.Lesson;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            return await _lessonRepository.CreateLessonAsync(lesson);
        }

        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            return await _lessonRepository.GetLessonByIdAsync(id);
        }

        public async Task<List<Lesson>> GetAllLessonsAsync()
        {
            return await _lessonRepository.GetAllLessonsAsync();
        }

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            return await _lessonRepository.UpdateLessonAsync(lesson);
        }

        public async Task DeleteLessonAsync(int id)
        {
            await _lessonRepository.DeleteLessonAsync(id);
        }
    }
}
