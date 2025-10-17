using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class LessonPlannerService : ILessonPlannerService
    {
        private readonly ILessonPlannerRepository _lessonPlannerRepo;

        public LessonPlannerService(ILessonPlannerRepository lessonPlannerRepo)
        {
            _lessonPlannerRepo = lessonPlannerRepo;
        }

        private LessonPlannerResponse MapToResponse(LessonPlanner planner)
        {
            if (planner == null) return null;
            return new LessonPlannerResponse
            {
                Id = planner.Id,
                Title = planner.Title,
                Content = planner.Content,
                Description = planner.Description,
                UserId = planner.UserId,
                ClassId = planner.ClassId,
                ClassName = planner.Class?.Name,
            };
        }

        private LessonPlanner MapToEntity(LessonPlannerRequest request, int userId, int? id = null)
        {
            var entity = new LessonPlanner
            {
                Title = request.Title?.Trim(),
                Content = request.Content?.Trim(),
                Description = request.Description?.Trim(),
                LessonNumber = request.LessonNumber?.Trim() ?? throw new ArgumentNullException(nameof(request.LessonNumber)),
                UserId = userId,
                ClassId = request.ClassId,
                DateOfPreparation = request.DateOfPreparation,
                DateOfTeaching = request.DateOfTeaching,
                UnitId = request.UnitId,
                LessonDefinitionId = request.LessonDefinitionId,
                MethodTemplateId = request.MethodTemplateId
            };
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }
        public async Task<LessonPlannerResponse> CreateLessonPlannerAsync(LessonPlannerRequest request, int currentUserId)
        {
            var lessonPlannerEntity = MapToEntity(request, currentUserId);
            var created = await _lessonPlannerRepo.CreateLessonPlannerAsync(lessonPlannerEntity);
            var createdWithDetails = await _lessonPlannerRepo.GetLessonPlannerByIdAsync(created.Id);
            return MapToResponse(createdWithDetails);
        }

        public async Task<LessonPlannerResponse> UpdateLessonPlannerAsync(int lessonPlannerId, LessonPlannerRequest request, int currentUserId)
        {
            var existing = await _lessonPlannerRepo.GetLessonPlannerByIdAsync(lessonPlannerId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"LessonPlanner with ID {lessonPlannerId} not found.");
            }
            if (existing.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own lesson planners.");
            }

            var lessonPlannerEntity = MapToEntity(request, currentUserId, lessonPlannerId);
            var updated = await _lessonPlannerRepo.UpdateLessonPlannerAsync(lessonPlannerEntity);
            var updatedWithDetails = await _lessonPlannerRepo.GetLessonPlannerByIdAsync(updated.Id);
            return MapToResponse(updatedWithDetails);
        }

        public async Task<LessonPlannerResponse> GetLessonPlannerByIdAsync(int id)
        {
            var planner = await _lessonPlannerRepo.GetLessonPlannerByIdAsync(id);
            if (planner == null)
            {
                throw new KeyNotFoundException($"LessonPlanner with ID {id} not found.");
            }
            return MapToResponse(planner);
        }

        public async Task<List<LessonPlannerResponse>> GetLessonPlannersByUserIdAsync(int userId)
        {
            var planners = await _lessonPlannerRepo.GetLessonPlannersByUserIdAsync(userId);
            return planners.Select(MapToResponse).ToList();
        }

        public async Task<List<LessonPlannerResponse>> GetAllLessonPlannersAsync()
        {
            var planners = await _lessonPlannerRepo.GetAllLessonPlannersAsync();
            return planners.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteLessonPlannerAsync(int id, int currentUserId)
        {
            var planner = await _lessonPlannerRepo.GetLessonPlannerByIdAsync(id);
            if (planner == null)
            {
                return false; // Or throw KeyNotFoundException
            }
            if (planner.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own lesson planners.");
            }
            return await _lessonPlannerRepo.DeleteLessonPlannerAsync(id);
        }
    }
}