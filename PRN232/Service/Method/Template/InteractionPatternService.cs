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
    public class InteractionPatternService : IInteractionPatternService
    {
        private readonly IInteractionPatternRepository _interactionPatternRepo;

        public InteractionPatternService(IInteractionPatternRepository interactionPatternRepo)
        {
            _interactionPatternRepo = interactionPatternRepo;
        }

        private InteractionPatternResponse MapToResponse(InteractionPattern entity)
        {
            if (entity == null) return null;
            return new InteractionPatternResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                ShortCode = entity.ShortCode,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            };
        }

        private InteractionPattern MapToEntity(InteractionPatternRequest request, int userId, int? id = null)
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Interaction pattern name is required and cannot be empty.");
            }

            var trimmedName = request.Name.Trim();
            if (trimmedName.Length > 200)
            {
                throw new ArgumentException("Interaction pattern name must be 200 characters or less.");
            }

            // Validate shortCode
            if (string.IsNullOrWhiteSpace(request.ShortCode))
            {
                throw new ArgumentException("Short code is required and cannot be empty.");
            }

            var trimmedShortCode = request.ShortCode.Trim();
            if (trimmedShortCode.Length > 10)
            {
                throw new ArgumentException("Short code must be 10 characters or less.");
            }

            // Validate description (optional)
            string trimmedDescription = null;
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                trimmedDescription = request.Description.Trim();
                if (trimmedDescription.Length > 2000)
                {
                    throw new ArgumentException("Description must be 2000 characters or less.");
                }
            }

            var entity = new InteractionPattern
            {
                UserId = userId,
                Name = trimmedName,
                ShortCode = trimmedShortCode,
                Description = trimmedDescription
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<InteractionPatternResponse> CreateAsync(InteractionPatternRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _interactionPatternRepo.CreateAsync(entity);
            var createdWithDetails = await _interactionPatternRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<InteractionPatternResponse> UpdateAsync(int interactionPatternId, InteractionPatternRequest request, int currentUserId)
        {
            var existing = await _interactionPatternRepo.GetByIdAsync(interactionPatternId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"InteractionPattern with ID {interactionPatternId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own interaction patterns.");
            }

            existing.Name = request.Name?.Trim();
            existing.ShortCode = request.ShortCode?.Trim();
            existing.Description = request.Description?.Trim();

            var updated = await _interactionPatternRepo.UpdateAsync(existing);
            var updatedWithDetails = await _interactionPatternRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<InteractionPatternResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _interactionPatternRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"InteractionPattern with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<InteractionPatternResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _interactionPatternRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _interactionPatternRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own interaction patterns.");
            }
            return await _interactionPatternRepo.DeleteAsync(id);
        }
    }
}
