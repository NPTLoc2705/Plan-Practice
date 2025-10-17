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
    public class GradeLevelService : IGradeLevelService
    {
        private readonly IGradeLevelRepository _gradeLevelRepo;
        public GradeLevelService(IGradeLevelRepository gradeLevelRepo)
        {
            _gradeLevelRepo = gradeLevelRepo;
        }
        private GradeLevelResponse MapToResponse(GradeLevel entity)
        {
            if (entity == null) return null;
            return new GradeLevelResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Level = entity.Level
            };
        }
        private GradeLevel MapToEntity(GradeLevelRequest request, int userId, int? id = null)
        {
            var entity = new GradeLevel
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Level = request.Level
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }
        public async Task<GradeLevelResponse> CreateAsync(GradeLevelRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _gradeLevelRepo.CreateAsync(entity);
            var createdWithDetails = await _gradeLevelRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }
        public async Task<GradeLevelResponse> UpdateAsync(int gradeLevelId, GradeLevelRequest request, int currentUserId)
        {
            var existing = await _gradeLevelRepo.GetByIdAsync(gradeLevelId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"GradeLevel with ID {gradeLevelId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own grade levels.");
            }
            existing.Name = request.Name?.Trim();
            existing.Level = request.Level;
            var updated = await _gradeLevelRepo.UpdateAsync(existing);
            var updatedWithDetails = await _gradeLevelRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }
        public async Task<GradeLevelResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _gradeLevelRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"GradeLevel with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }
        public async Task<List<GradeLevelResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _gradeLevelRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }
        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _gradeLevelRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own grade levels.");
            }
            return await _gradeLevelRepo.DeleteAsync(id);
        }
    }
}