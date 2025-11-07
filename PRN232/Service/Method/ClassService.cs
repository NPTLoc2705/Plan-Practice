using BusinessObject;
using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using Repository.Interface; // Import repository interface namespace
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Method
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepo;
        private readonly IGradeLevelRepository _gradeLevelRepo;

        public ClassService(IClassRepository classRepo, IGradeLevelRepository gradeLevelRepo)
        {
            _classRepo = classRepo;
            _gradeLevelRepo = gradeLevelRepo;
        }

        private ClassResponse MapToResponse(Class entity)
        {
            if (entity == null) return null;
            return new ClassResponse
            {
                Id = entity.Id,
                UserId = entity.GradeLevel.UserId,
                Name = entity.Name,
                GradeLevelId = entity.GradeLevelId,
                GradeLevelName = entity.GradeLevel?.Name
            };
        }

        private Class MapToEntity(ClassRequest request, int userId, int? id = null)
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Class name is required and cannot be empty.");
            }

            var trimmedName = request.Name.Trim();
            if (trimmedName.Length > 200)
            {
                throw new ArgumentException("Class name must be 200 characters or less.");
            }

            // Validate gradeLevelId
            if (request.GradeLevelId <= 0)
            {
                throw new ArgumentException("Valid grade level is required.");
            }

            var entity = new Class
            {
                Name = trimmedName,
                GradeLevelId = request.GradeLevelId
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<ClassResponse> CreateAsync(ClassRequest request, int userId)
        {
            // ✅ FIXED: Pass both id and userId
            var gradeLevel = await _gradeLevelRepo.GetByIdAsync(request.GradeLevelId, userId);
            if (gradeLevel == null || gradeLevel.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only create a class for a grade level you own.");
            }

            var entity = MapToEntity(request, userId);
            var created = await _classRepo.CreateAsync(entity);
            var createdWithDetails = await _classRepo.GetByIdAsync(created.Id, userId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<ClassResponse> UpdateAsync(int id, ClassRequest request, int userId)
        {
            var existing = await _classRepo.GetByIdAsync(id, userId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Class with ID {id} not found.");
            }

            // ✅ FIXED: Pass both id and userId
            var gradeLevel = await _gradeLevelRepo.GetByIdAsync(request.GradeLevelId, userId);
            if (gradeLevel == null || gradeLevel.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only assign a class to a grade level you own.");
            }

            existing.Name = request.Name?.Trim();
            existing.GradeLevelId = request.GradeLevelId;

            var updated = await _classRepo.UpdateAsync(existing);
            var updatedWithDetails = await _classRepo.GetByIdAsync(updated.Id, userId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<ClassResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _classRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Class with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<ClassResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _classRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var existing = await _classRepo.GetByIdAsync(id, userId);
            if (existing == null)
            {
                return false;
            }
            return await _classRepo.DeleteAsync(id);
        }

        public async Task<List<ClassResponse>> GetAllByGradeLevelIdAsync(int gradeLevelId, int userId)
        {
            // ✅ FIXED: Pass both id and userId
            var gradeLevel = await _gradeLevelRepo.GetByIdAsync(gradeLevelId, userId);
            if (gradeLevel == null || gradeLevel.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to view classes for this grade level.");
            }

            var entities = await _classRepo.GetAllByGradeLevelIdAsync(gradeLevelId, userId);
            return entities.Select(MapToResponse).ToList();
        }
    }
}