using BusinessObject.Dtos.LessonDTO;
using BusinessObject.Lesson;
using BusinessObject.Lesson.Template;
using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Enhanced LessonPlanner Service with History/Snapshot Support
    /// Preserves data even when referenced templates are deleted
    /// </summary>
    public class LessonPlannerServiceEnhanced : ILessonPlannerService
    {
        private readonly ILessonPlannerRepository _lessonPlannerRepo;
        private readonly PlantPraticeDbContext _context;

        public LessonPlannerServiceEnhanced(ILessonPlannerRepository lessonPlannerRepo, PlantPraticeDbContext context)
        {
            _lessonPlannerRepo = lessonPlannerRepo;
            _context = context;
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
                ClassName = planner.Class?.Name ?? planner.SnapshotClassName,
            };
        }

        private async Task<LessonPlanner> MapToEntityWithSnapshotsAsync(LessonPlannerRequest request, int userId, int? id = null)
        {
            var entity = new LessonPlanner
            {
                Title = request.Title?.Trim(),
                Content = request.Content?.Trim(),
                Description = request.Description?.Trim(),
                LessonNumber = request.LessonNumber?.Trim(),
                UserId = userId,
                ClassId = request.ClassId,
                DateOfPreparation = request.DateOfPreparation,
                DateOfTeaching = request.DateOfTeaching,
                UnitId = request.UnitId,
                LessonDefinitionId = request.LessonDefinitionId,
                MethodTemplateId = request.MethodTemplateId,
                Objectives = new List<LessonObjective>(),
                Skills = new List<LessonSkill>(),
                Attitudes = new List<LessonAttitude>(),
                LanguageFocusItems = new List<LessonLanguageFocus>(),
                Preparations = new List<LessonPreparation>(),
                ActivityStages = new List<LessonActivityStage>()
            };

            if (id.HasValue)
            {
                entity.Id = id.Value;
            }

            // Populate snapshot data for Class and GradeLevel
            var classEntity = await _context.Classes
                .Include(c => c.GradeLevel)
                .FirstOrDefaultAsync(c => c.Id == request.ClassId);
            
            if (classEntity != null)
            {
                entity.SnapshotClassName = classEntity.Name;
                entity.SnapshotGradeLevelName = classEntity.GradeLevel?.Name;
            }

            // Populate snapshot data for Method Template
            if (request.MethodTemplateId.HasValue)
            {
                var methodTemplate = await _context.MethodTemplates
                    .FirstOrDefaultAsync(m => m.Id == request.MethodTemplateId);
                
                if (methodTemplate != null)
                {
                    entity.SnapshotMethodName = methodTemplate.Name;
                    entity.SnapshotMethodDescription = methodTemplate.Description;
                }
                else
                {
                    // Template doesn't exist, set to null to avoid FK constraint violation
                    entity.MethodTemplateId = null;
                }
            }

            // Map Objectives with snapshot data
            foreach (var objDto in request.Objectives ?? new List<LessonObjectiveDto>())
            {
                var lessonObj = new LessonObjective
                {
                    CustomContent = objDto.CustomContent,
                    DisplayOrder = objDto.DisplayOrder
                };

                // Populate snapshot data and validate template exists
                if (objDto.ObjectiveTemplateId.HasValue)
                {
                    var template = await _context.ObjectiveTemplates
                        .FirstOrDefaultAsync(t => t.Id == objDto.ObjectiveTemplateId);
                    
                    if (template != null)
                    {
                        lessonObj.ObjectiveTemplateId = objDto.ObjectiveTemplateId;
                        lessonObj.SnapshotName = template.Name;
                        lessonObj.SnapshotContent = template.Content;
                    }
                    // else: template doesn't exist, leave ObjectiveTemplateId as null
                }

                entity.Objectives.Add(lessonObj);
            }

            // Map Skills with snapshot data
            foreach (var skillDto in request.Skills ?? new List<LessonSkillDto>())
            {
                var lessonSkill = new LessonSkill
                {
                    CustomContent = skillDto.Description,
                    DisplayOrder = skillDto.DisplayOrder
                };

                // Populate snapshot data and validate template exists
                if (skillDto.SkillTemplateId.HasValue)
                {
                    var template = await _context.SkillTemplates
                        .Include(st => st.SkillType)
                        .FirstOrDefaultAsync(t => t.Id == skillDto.SkillTemplateId);
                    
                    if (template != null)
                    {
                        lessonSkill.SkillTemplateId = skillDto.SkillTemplateId;
                        lessonSkill.SnapshotName = template.Name;
                        lessonSkill.SnapshotDescription = template.Description;
                        lessonSkill.SnapshotSkillType = template.SkillType?.Name;
                    }
                    // else: template doesn't exist, leave SkillTemplateId as null
                }

                entity.Skills.Add(lessonSkill);
            }

            // Map Attitudes with snapshot data
            foreach (var attDto in request.Attitudes ?? new List<LessonAttitudeDto>())
            {
                var lessonAtt = new LessonAttitude
                {
                    CustomContent = attDto.CustomContent,
                    DisplayOrder = attDto.DisplayOrder
                };

                // Populate snapshot data and validate template exists
                if (attDto.AttitudeTemplateId.HasValue)
                {
                    var template = await _context.AttitudeTemplates
                        .FirstOrDefaultAsync(t => t.Id == attDto.AttitudeTemplateId);
                    
                    if (template != null)
                    {
                        lessonAtt.AttitudeTemplateId = attDto.AttitudeTemplateId;
                        lessonAtt.SnapshotName = template.Name;
                        lessonAtt.SnapshotContent = template.Content;
                    }
                    // else: template doesn't exist, leave AttitudeTemplateId as null
                }

                entity.Attitudes.Add(lessonAtt);
            }

            // Map Language Focus with snapshot data
            foreach (var lfDto in request.LanguageFocusItems ?? new List<LessonLanguageFocusDto>())
            {
                var lessonLF = new LessonLanguageFocus
                {
                    Content = lfDto.Content,
                    DisplayOrder = lfDto.DisplayOrder
                };

                // Populate snapshot data and validate type exists
                if (lfDto.LanguageFocusTypeId.HasValue)
                {
                    var type = await _context.LanguageFocusTypes
                        .FirstOrDefaultAsync(t => t.Id == lfDto.LanguageFocusTypeId);
                    
                    if (type != null)
                    {
                        lessonLF.LanguageFocusTypeId = lfDto.LanguageFocusTypeId;
                        lessonLF.SnapshotTypeName = type.Name;
                    }
                    // else: type doesn't exist, leave LanguageFocusTypeId as null
                }

                entity.LanguageFocusItems.Add(lessonLF);
            }

            // Map Preparations with snapshot data
            foreach (var prepDto in request.Preparations ?? new List<LessonPreparationDto>())
            {
                var lessonPrep = new LessonPreparation
                {
                    Materials = prepDto.Materials,
                    DisplayOrder = prepDto.DisplayOrder
                };

                // Populate snapshot data and validate type exists
                if (prepDto.PreparationTypeId.HasValue)
                {
                    var type = await _context.PreparationTypes
                        .FirstOrDefaultAsync(t => t.Id == prepDto.PreparationTypeId);
                    
                    if (type != null)
                    {
                        lessonPrep.PreparationTypeId = prepDto.PreparationTypeId;
                        lessonPrep.SnapshotTypeName = type.Name;
                    }
                    // else: type doesn't exist, leave PreparationTypeId as null
                }

                entity.Preparations.Add(lessonPrep);
            }

            // Map Activity Stages and Items with snapshot data
            foreach (var stageDto in request.ActivityStages ?? new List<LessonActivityStageDto>())
            {
                var stage = new LessonActivityStage
                {
                    StageName = stageDto.StageName,
                    DisplayOrder = stageDto.DisplayOrder,
                    ActivityItems = new List<LessonActivityItem>()
                };

                foreach (var itemDto in stageDto.ActivityItems ?? new List<LessonActivityItemDto>())
                {
                    var activityItem = new LessonActivityItem
                    {
                        TimeInMinutes = itemDto.TimeInMinutes,
                        Content = itemDto.Content,
                        DisplayOrder = itemDto.DisplayOrder
                    };

                    // Populate snapshot data and validate pattern exists
                    if (itemDto.InteractionPatternId.HasValue)
                    {
                        var pattern = await _context.InteractionPatterns
                            .FirstOrDefaultAsync(p => p.Id == itemDto.InteractionPatternId);
                        
                        if (pattern != null)
                        {
                            activityItem.InteractionPatternId = itemDto.InteractionPatternId;
                            activityItem.SnapshotInteractionName = pattern.Name;
                            activityItem.SnapshotInteractionShortCode = pattern.ShortCode;
                        }
                        // else: pattern doesn't exist, leave InteractionPatternId as null
                    }

                    stage.ActivityItems.Add(activityItem);
                }

                entity.ActivityStages.Add(stage);
            }

            return entity;
        }

        public async Task<LessonPlannerResponse> CreateLessonPlannerAsync(LessonPlannerRequest request, int currentUserId)
        {
            var lessonPlannerEntity = await MapToEntityWithSnapshotsAsync(request, currentUserId);
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

            var lessonPlannerEntity = await MapToEntityWithSnapshotsAsync(request, currentUserId, lessonPlannerId);
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
                return false;
            }
            if (planner.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own lesson planners.");
            }
            return await _lessonPlannerRepo.DeleteLessonPlannerAsync(id);
        }
    }
}
