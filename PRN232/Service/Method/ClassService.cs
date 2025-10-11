using BusinessObject.Dtos.LessonDTO;
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
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepo;
        private readonly IGradeLevelRepository _gradeLevelRepo; // To validate GradeLevelId

        public ClassService(IClassRepository classRepo, IGradeLevelRepository gradeLevelRepo)
        {
            _classRepo = classRepo;
            _gradeLevelRepo = gradeLevelRepo;
        }

        private ClassResponse MapToResponse(Class entity) => new ClassResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            GradeLevelId = entity.GradeLevelId,
            GradeLevelName = entity.GradeLevel?.Name
        };

        public async Task<ClassResponse> CreateAsync(ClassRequest request)
        {
            var gradeLevelExists = await _gradeLevelRepo.GetByIdAsync(request.GradeLevelId);
            if (gradeLevelExists == null)
                throw new KeyNotFoundException($"GradeLevel with ID {request.GradeLevelId} not found.");

            var entity = new Class { Name = request.Name, GradeLevelId = request.GradeLevelId };
            var created = await _classRepo.CreateAsync(entity);

            // Re-fetch to populate navigation property for the response
            var createdWithDetails = await _classRepo.GetByIdAsync(created.Id);
            return MapToResponse(createdWithDetails);
        }

        public async Task<ClassResponse> GetByIdAsync(int id)
        {
            var entity = await _classRepo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Class with ID {id} not found.");
            return MapToResponse(entity);
        }

        public async Task<List<ClassResponse>> GetAllAsync()
        {
            var entities = await _classRepo.GetAllAsync();
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<ClassResponse> UpdateAsync(int id, ClassRequest request)
        {
            var existing = await _classRepo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Class with ID {id} not found.");

            var gradeLevelExists = await _gradeLevelRepo.GetByIdAsync(request.GradeLevelId);
            if (gradeLevelExists == null)
                throw new KeyNotFoundException($"GradeLevel with ID {request.GradeLevelId} not found.");

            existing.Name = request.Name;
            existing.GradeLevelId = request.GradeLevelId;

            var updated = await _classRepo.UpdateAsync(existing);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _classRepo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Class with ID {id} not found.");

            return await _classRepo.DeleteAsync(id);
        }
    }
}
