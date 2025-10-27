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
    public class SkillTemplateService : ISkillTemplateService
    {
        private readonly ISkillTemplateRepository _skillTemplateRepo;

        public SkillTemplateService(ISkillTemplateRepository skillTemplateRepo)
        {
            _skillTemplateRepo = skillTemplateRepo;
        }

        private SkillTemplateResponse MapToResponse(SkillTemplate entity)
        {
            if (entity == null) return null;
            return new SkillTemplateResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                CreatedAt = entity.CreatedAt
            };
        }

        private SkillTemplate MapToEntity(SkillTemplateRequest request, int userId, int? id = null)
        {
            var entity = new SkillTemplate
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

        public async Task<SkillTemplateResponse> CreateAsync(SkillTemplateRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _skillTemplateRepo.CreateAsync(entity);
            var createdWithDetails = await _skillTemplateRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<SkillTemplateResponse> UpdateAsync(int skillTemplateId, SkillTemplateRequest request, int currentUserId)
        {
            var existing = await _skillTemplateRepo.GetByIdAsync(skillTemplateId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"SkillTemplate with ID {skillTemplateId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own skill templates.");
            }

            existing.Name = request.Name?.Trim();
            existing.Description = request.Description?.Trim();
            existing.DisplayOrder = request.DisplayOrder;

            var updated = await _skillTemplateRepo.UpdateAsync(existing);
            var updatedWithDetails = await _skillTemplateRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<SkillTemplateResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _skillTemplateRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"SkillTemplate with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<SkillTemplateResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _skillTemplateRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _skillTemplateRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own skill templates.");
            }
            return await _skillTemplateRepo.DeleteAsync(id);
        }
    }
}
