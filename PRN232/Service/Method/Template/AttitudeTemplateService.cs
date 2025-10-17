using BusinessObject.Dtos.LessonDTO.TemplateDTO;
using BusinessObject.Lesson.Template;
using Repository.Interface.Template;
using Service.Interface.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method.Template
{
    public class AttitudeTemplateService : IAttitudeTemplateService
    {
        private readonly IAttitudeTemplateRepository _attitudeTemplateRepo;

        public AttitudeTemplateService(IAttitudeTemplateRepository attitudeTemplateRepo)
        {
            _attitudeTemplateRepo = attitudeTemplateRepo;
        }

        private AttitudeTemplateResponse MapToResponse(AttitudeTemplate entity)
        {
            if (entity == null) return null;
            return new AttitudeTemplateResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Content = entity.Content,
                DisplayOrder = entity.DisplayOrder,
                CreatedAt = entity.CreatedAt
            };
        }

        private AttitudeTemplate MapToEntity(AttitudeTemplateRequest request, int userId, int? id = null)
        {
            var entity = new AttitudeTemplate
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Content = request.Content?.Trim() ?? throw new ArgumentNullException(nameof(request.Content)),
                DisplayOrder = request.DisplayOrder
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<AttitudeTemplateResponse> CreateAsync(AttitudeTemplateRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _attitudeTemplateRepo.CreateAsync(entity);
            var createdWithDetails = await _attitudeTemplateRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<AttitudeTemplateResponse> UpdateAsync(int attitudeTemplateId, AttitudeTemplateRequest request, int currentUserId)
        {
            var existing = await _attitudeTemplateRepo.GetByIdAsync(attitudeTemplateId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"AttitudeTemplate with ID {attitudeTemplateId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own attitude templates.");
            }

            existing.Name = request.Name?.Trim();
            existing.Content = request.Content?.Trim();
            existing.DisplayOrder = request.DisplayOrder;

            var updated = await _attitudeTemplateRepo.UpdateAsync(existing);
            var updatedWithDetails = await _attitudeTemplateRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<AttitudeTemplateResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _attitudeTemplateRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"AttitudeTemplate with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<AttitudeTemplateResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _attitudeTemplateRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _attitudeTemplateRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own attitude templates.");
            }
            return await _attitudeTemplateRepo.DeleteAsync(id);
        }
    }
}
