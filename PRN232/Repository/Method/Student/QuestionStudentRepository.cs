using BusinessObject.Quiz;
using DAL;
using Repository.Interface;
using Repository.Interface.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Student;


namespace Repository.Method.Student
{
    public class QuestionStudentRepository : IQuestionStudentRepository
    {
        private readonly QuestionStudentDAO _questionStudentDAO;
        public QuestionStudentRepository(QuestionStudentDAO questionStudentDAO) { 
        this._questionStudentDAO = questionStudentDAO;
        }
        public Task<int> GetQuestionCountByQuizIdAsync(int quizId) => _questionStudentDAO.GetQuestionCountByQuizIdAsync(quizId);



   

    }
}