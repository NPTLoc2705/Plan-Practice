using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Lesson;
namespace DAL.LessonDAO
{
    public class LessonDAO
    {
       private readonly PlantPraticeDbContext _context;
        public  LessonDAO(PlantPraticeDbContext content)
        {
            _context = content;
        }
        //public async Task<Lesson> CreateLesson(Lesson lesson)
        //{

        //}
    }
}
