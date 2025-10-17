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
    public class PreparationTypeService : IPreparationTypeService
    {
        private readonly IPreparationTypeRepository _preparationTypeRepo;

        public PreparationTypeService(IPreparationTypeRepository preparationTypeRepo)
        {
            _preparationTypeRepo = preparationTypeRepo;
        }

        private PreparationTypeResponse MapToResponse(PreparationType entity)
        {
            if (entity == null) return null;
            return new PreparationTypeResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            };
        }

        private PreparationType MapToEntity(PreparationTypeRequest request, int userId, int? id = null)
        {
            var entity = new PreparationType
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Description = request.Description?.Trim()
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<PreparationTypeResponse> CreateAsync(PreparationTypeRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _preparationTypeRepo.CreateAsync(entity);
            var createdWithDetails = await _preparationTypeRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<PreparationTypeResponse> UpdateAsync(int preparationTypeId, PreparationTypeRequest request, int currentUserId)
        {
            var existing = await _preparationTypeRepo.GetByIdAsync(preparationTypeId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"PreparationType with ID {preparationTypeId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own preparation types.");
            }

            existing.Name = request.Name?.Trim();
            existing.Description = request.Description?.Trim();

            var updated = await _preparationTypeRepo.UpdateAsync(existing);
            var updatedWithDetails = await _preparationTypeRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<PreparationTypeResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _preparationTypeRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"PreparationType with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<PreparationTypeResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _preparationTypeRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _preparationTypeRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own preparation types.");
            }
            return await _preparationTypeRepo.DeleteAsync(id);
        }
    }
}
