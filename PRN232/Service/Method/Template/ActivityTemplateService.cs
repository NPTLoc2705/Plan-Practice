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
    public class ActivityTemplateService : IActivityTemplateService
    {
        private readonly IActivityTemplateRepository _activityTemplateRepo;

        public ActivityTemplateService(IActivityTemplateRepository activityTemplateRepo)
        {
            _activityTemplateRepo = activityTemplateRepo;
        }

        private ActivityTemplateResponse MapToResponse(ActivityTemplate entity)
        {
            if (entity == null) return null;
            return new ActivityTemplateResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Description = entity.Description,
                Content = entity.Content,
                CreatedAt = entity.CreatedAt
            };
        }

        private ActivityTemplate MapToEntity(ActivityTemplateRequest request, int userId, int? id = null)
        {
            var entity = new ActivityTemplate
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Description = request.Description?.Trim(),
                Content = request.Content?.Trim() ?? throw new ArgumentNullException(nameof(request.Content))
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<ActivityTemplateResponse> CreateAsync(ActivityTemplateRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _activityTemplateRepo.CreateAsync(entity);
            var createdWithDetails = await _activityTemplateRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<ActivityTemplateResponse> UpdateAsync(int activityTemplateId, ActivityTemplateRequest request, int currentUserId)
        {
            var existing = await _activityTemplateRepo.GetByIdAsync(activityTemplateId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"ActivityTemplate with ID {activityTemplateId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own activity templates.");
            }

            existing.Name = request.Name?.Trim();
            existing.Description = request.Description?.Trim();
            existing.Content = request.Content?.Trim();

            var updated = await _activityTemplateRepo.UpdateAsync(existing);
            var updatedWithDetails = await _activityTemplateRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<ActivityTemplateResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _activityTemplateRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"ActivityTemplate with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<ActivityTemplateResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _activityTemplateRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _activityTemplateRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own activity templates.");
            }
            return await _activityTemplateRepo.DeleteAsync(id);
        }
    }
}
