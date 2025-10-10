using BusinessObject.Quiz;
using DAL.Student;
using Repository.Interface.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Student
{
    public class UserAnswerStudentRepository : IUserAnswerStudentRepository
    {
        private readonly UserAnswerStudentDAO userAnswerStudentDAO;

        public UserAnswerStudentRepository(UserAnswerStudentDAO userAnserStudentDAO)
        {
            this.userAnswerStudentDAO = userAnserStudentDAO;
        }

        public Task CreateUserAnswersAsync(IEnumerable<UserAnswer> userAnswers)
        => userAnswerStudentDAO.CreateUserAnswersAsync(userAnswers);


    }
}