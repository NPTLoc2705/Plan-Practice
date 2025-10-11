using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method
{
    public class GradeLevelService : IGradeLevelService
    {
        private readonly IGradeLevelRepository _repo;

        public GradeLevelService(IGradeLevelRepository repo)
        {
            _repo = repo;
        }

        private GradeLevelResponse MapToResponse(GradeLevel entity) => new GradeLevelResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Level = entity.Level
        };

        private GradeLevel MapToEntity(GradeLevelRequest dto) => new GradeLevel
        {
            Name = dto.Name,
            Level = dto.Level
        };

        public async Task<GradeLevelResponse> CreateAsync(GradeLevelRequest request)
        {
            var entity = MapToEntity(request);
            var created = await _repo.CreateAsync(entity);
            return MapToResponse(created);
        }

        public async Task<GradeLevelResponse> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"GradeLevel with ID {id} not found.");
            return MapToResponse(entity);
        }

        public async Task<List<GradeLevelResponse>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(MapToResponse).ToList();
        }

        public async Task<GradeLevelResponse> UpdateAsync(int id, GradeLevelRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"GradeLevel with ID {id} not found.");

            existing.Name = request.Name;
            existing.Level = request.Level;

            var updated = await _repo.UpdateAsync(existing);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"GradeLevel with ID {id} not found.");

            return await _repo.DeleteAsync(id);
        }
    }
}
