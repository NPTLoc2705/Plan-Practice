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
    public class LanguageFocusTypeService : ILanguageFocusTypeService
    {
        private readonly ILanguageFocusTypeRepository _languageFocusTypeRepo;

        public LanguageFocusTypeService(ILanguageFocusTypeRepository languageFocusTypeRepo)
        {
            _languageFocusTypeRepo = languageFocusTypeRepo;
        }

        private LanguageFocusTypeResponse MapToResponse(LanguageFocusType entity)
        {
            if (entity == null) return null;
            return new LanguageFocusTypeResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                CreatedAt = entity.CreatedAt
            };
        }

        private LanguageFocusType MapToEntity(LanguageFocusTypeRequest request, int userId, int? id = null)
        {
            var entity = new LanguageFocusType
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Description = request.Description?.Trim(),
                DisplayOrder = request.DisplayOrder
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<LanguageFocusTypeResponse> CreateAsync(LanguageFocusTypeRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _languageFocusTypeRepo.CreateAsync(entity);
            var createdWithDetails = await _languageFocusTypeRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<LanguageFocusTypeResponse> UpdateAsync(int languageFocusTypeId, LanguageFocusTypeRequest request, int currentUserId)
        {
            var existing = await _languageFocusTypeRepo.GetByIdAsync(languageFocusTypeId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"LanguageFocusType with ID {languageFocusTypeId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own language focus types.");
            }

            existing.Name = request.Name?.Trim();
            existing.Description = request.Description?.Trim();
            existing.DisplayOrder = request.DisplayOrder;

            var updated = await _languageFocusTypeRepo.UpdateAsync(existing);
            var updatedWithDetails = await _languageFocusTypeRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<LanguageFocusTypeResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _languageFocusTypeRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"LanguageFocusType with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<LanguageFocusTypeResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _languageFocusTypeRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _languageFocusTypeRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own language focus types.");
            }
            return await _languageFocusTypeRepo.DeleteAsync(id);
        }
    }
}
