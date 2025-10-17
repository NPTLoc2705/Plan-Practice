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
    public class SkillTypeService : ISkillTypeService
    {
        private readonly ISkillTypeRepository _skillTypeRepo;

        public SkillTypeService(ISkillTypeRepository skillTypeRepo)
        {
            _skillTypeRepo = skillTypeRepo;
        }

        private SkillTypeResponse MapToResponse(SkillType skillType)
        {
            if (skillType == null) return null;
            return new SkillTypeResponse
            {
                Id = skillType.Id,
                UserId = skillType.UserId,
                Name = skillType.Name,
                Description = skillType.Description,
                CreatedAt = skillType.CreatedAt
            };
        }

        private SkillType MapToEntity(SkillTypeRequest request, int userId, int? id = null)
        {
            var entity = new SkillType
            {
                UserId = userId,
                Name = request.Name?.Trim() ?? throw new ArgumentNullException(nameof(request.Name)),
                Description = request.Description?.Trim(),
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public async Task<SkillTypeResponse> CreateSkillTypeAsync(SkillTypeRequest request, int currentUserId)
        {
            var skillTypeEntity = MapToEntity(request, currentUserId);
            var created = await _skillTypeRepo.CreateSkillTypeAsync(skillTypeEntity);
            var createdWithDetails = await _skillTypeRepo.GetSkillTypeByIdAsync(created.Id);
            return MapToResponse(createdWithDetails);
        }

        public async Task<SkillTypeResponse> UpdateSkillTypeAsync(int skillTypeId, SkillTypeRequest request, int currentUserId)
        {
            var existing = await _skillTypeRepo.GetSkillTypeByIdAsync(skillTypeId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"SkillType with ID {skillTypeId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own skill types.");
            }

            var skillTypeEntity = MapToEntity(request, currentUserId, skillTypeId);
            var updated = await _skillTypeRepo.UpdateSkillTypeAsync(skillTypeEntity);
            var updatedWithDetails = await _skillTypeRepo.GetSkillTypeByIdAsync(updated.Id);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<SkillTypeResponse> GetSkillTypeByIdAsync(int id)
        {
            var skillType = await _skillTypeRepo.GetSkillTypeByIdAsync(id);
            if (skillType == null)
            {
                throw new KeyNotFoundException($"SkillType with ID {id} not found.");
            }
            return MapToResponse(skillType);
        }

        public async Task<List<SkillTypeResponse>> GetSkillTypesByUserIdAsync(int userId)
        {
            var skillTypes = await _skillTypeRepo.GetSkillTypesByUserIdAsync(userId);
            return skillTypes.Select(MapToResponse).ToList();
        }

        public async Task<List<SkillTypeResponse>> GetAllSkillTypesAsync()
        {
            var skillTypes = await _skillTypeRepo.GetAllSkillTypesAsync();
            return skillTypes.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteSkillTypeAsync(int id, int currentUserId)
        {
            var skillType = await _skillTypeRepo.GetSkillTypeByIdAsync(id);
            if (skillType == null)
            {
                return false;
            }
            if (skillType.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own skill types.");
            }
            return await _skillTypeRepo.DeleteSkillTypeAsync(id);
        }
    }
}
