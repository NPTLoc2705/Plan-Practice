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
    public class ObjectiveTemplateService : IObjectiveTemplateService
    {
        private readonly IObjectiveTemplateRepository _objectiveTemplateRepo;

        public ObjectiveTemplateService(IObjectiveTemplateRepository objectiveTemplateRepo)
        {
            _objectiveTemplateRepo = objectiveTemplateRepo;
        }

        private ObjectiveTemplateResponse MapToResponse(ObjectiveTemplate entity)
        {
            if (entity == null) return null;
            return new ObjectiveTemplateResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Content = entity.Content,
                // ✅ REMOVED: Category
                DisplayOrder = entity.DisplayOrder,
                CreatedAt = entity.CreatedAt
            };
        }

        private ObjectiveTemplate MapToEntity(ObjectiveTemplateRequest request, int userId, int? id = null)
        {
            var entity = new ObjectiveTemplate
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Content = request.Content?.Trim() ?? throw new ArgumentNullException(nameof(request.Content)),
                // ✅ REMOVED: Category
                DisplayOrder = request.DisplayOrder
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        // ✅ REST SAME AS BEFORE - Create, Update, Get, Delete methods unchanged
        public async Task<ObjectiveTemplateResponse> CreateAsync(ObjectiveTemplateRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _objectiveTemplateRepo.CreateAsync(entity);
            var createdWithDetails = await _objectiveTemplateRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<ObjectiveTemplateResponse> UpdateAsync(int objectiveTemplateId, ObjectiveTemplateRequest request, int currentUserId)
        {
            var existing = await _objectiveTemplateRepo.GetByIdAsync(objectiveTemplateId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"ObjectiveTemplate with ID {objectiveTemplateId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own objective templates.");
            }

            existing.Name = request.Name?.Trim();
            existing.Content = request.Content?.Trim();
            // ✅ REMOVED: Category
            existing.DisplayOrder = request.DisplayOrder;

            var updated = await _objectiveTemplateRepo.UpdateAsync(existing);
            var updatedWithDetails = await _objectiveTemplateRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<ObjectiveTemplateResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _objectiveTemplateRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"ObjectiveTemplate with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<ObjectiveTemplateResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _objectiveTemplateRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _objectiveTemplateRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own objective templates.");
            }
            return await _objectiveTemplateRepo.DeleteAsync(id);
        }
    }
}
