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
    public class MethodTemplateService : IMethodTemplateService
    {
        private readonly IMethodTemplateRepository _methodTemplateRepo;

        public MethodTemplateService(IMethodTemplateRepository methodTemplateRepo)
        {
            _methodTemplateRepo = methodTemplateRepo;
        }

        private MethodTemplateResponse MapToResponse(MethodTemplate entity)
        {
            if (entity == null) return null;
            return new MethodTemplateResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            };
        }

        private MethodTemplate MapToEntity(MethodTemplateRequest request, int userId, int? id = null)
        {
            var entity = new MethodTemplate
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

        public async Task<MethodTemplateResponse> CreateAsync(MethodTemplateRequest request, int currentUserId)
        {
            var entity = MapToEntity(request, currentUserId);
            var created = await _methodTemplateRepo.CreateAsync(entity);
            var createdWithDetails = await _methodTemplateRepo.GetByIdAsync(created.Id, currentUserId);
            return MapToResponse(createdWithDetails);
        }

        public async Task<MethodTemplateResponse> UpdateAsync(int methodTemplateId, MethodTemplateRequest request, int currentUserId)
        {
            var existing = await _methodTemplateRepo.GetByIdAsync(methodTemplateId, currentUserId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"MethodTemplate with ID {methodTemplateId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own method templates.");
            }

            existing.Name = request.Name?.Trim();
            existing.Description = request.Description?.Trim();

            var updated = await _methodTemplateRepo.UpdateAsync(existing);
            var updatedWithDetails = await _methodTemplateRepo.GetByIdAsync(updated.Id, currentUserId);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<MethodTemplateResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _methodTemplateRepo.GetByIdAsync(id, userId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"MethodTemplate with ID {id} not found for this user.");
            }
            return MapToResponse(entity);
        }

        public async Task<List<MethodTemplateResponse>> GetAllByUserIdAsync(int userId)
        {
            var entities = await _methodTemplateRepo.GetAllByUserIdAsync(userId);
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _methodTemplateRepo.GetByIdAsync(id, currentUserId);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own method templates.");
            }
            return await _methodTemplateRepo.DeleteAsync(id); // ✅ FIXED: Works perfectly!
        }
    }
}
