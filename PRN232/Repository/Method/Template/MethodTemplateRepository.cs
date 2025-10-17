using BusinessObject.Lesson.Template;
using DAL.LessonDAO.Template;
using Repository.Interface.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Template
{
    public class MethodTemplateRepository : IMethodTemplateRepository
    {
        private readonly MethodTemplateDAO _methodTemplateDAO;

        public MethodTemplateRepository(MethodTemplateDAO methodTemplateDAO)
        {
            _methodTemplateDAO = methodTemplateDAO;
        }

        public async Task<MethodTemplate> CreateAsync(MethodTemplate methodTemplate)
        {
            return await _methodTemplateDAO.CreateAsync(methodTemplate);
        }

        public async Task<MethodTemplate> GetByIdAsync(int id, int userId)
        {
            return await _methodTemplateDAO.GetByIdAsync(id, userId);
        }

        public async Task<List<MethodTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await _methodTemplateDAO.GetAllByUserIdAsync(userId);
        }

        public async Task<MethodTemplate> UpdateAsync(MethodTemplate methodTemplate)
        {
            var existing = await _methodTemplateDAO.GetByIdAsync(methodTemplate.Id, methodTemplate.UserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"MethodTemplate with ID {methodTemplate.Id} not found.");
            }
            return await _methodTemplateDAO.UpdateAsync(methodTemplate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _methodTemplateDAO.DeleteAsync(id); // ✅ FIXED: No userId=0!
        }
    }
}
