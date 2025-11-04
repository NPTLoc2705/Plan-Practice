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
    public class LessonPlannerDocumentService : ILessonPlannerDocumentService
    {
        private readonly ILessonPlannerDocumentRepository _documentRepository;

        public LessonPlannerDocumentService(ILessonPlannerDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        private LessonPlannerDocumentResponse MapToResponse(LessonPlannerDocument document)
        {
            if (document == null) return null;
            return new LessonPlannerDocumentResponse
            {
                Id = document.Id,
                FilePath = document.FilePath,
                LessonPlannerId = document.LessonPlannerId,
                CreatedAt = document.CreatedAt
            };
        }

        private LessonPlannerDocument MapToEntity(LessonPlannerDocumentRequest request)
        {
            return new LessonPlannerDocument
            {
                FilePath = request.FilePath,
                LessonPlannerId = request.LessonPlannerId
            };
        }

        public async Task<LessonPlannerDocumentResponse> CreateDocumentAsync(LessonPlannerDocumentRequest request)
        {
            var documentEntity = MapToEntity(request);
            var created = await _documentRepository.CreateDocumentAsync(documentEntity);
            return MapToResponse(created);
        }

        public async Task<LessonPlannerDocumentResponse> GetDocumentByIdAsync(int id)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(id);
            if (document == null)
            {
                throw new KeyNotFoundException($"Document with ID {id} not found.");
            }
            return MapToResponse(document);
        }

        public async Task<List<LessonPlannerDocumentResponse>> GetDocumentsByLessonPlannerIdAsync(int lessonPlannerId)
        {
            var documents = await _documentRepository.GetDocumentsByLessonPlannerIdAsync(lessonPlannerId);
            return documents.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            return await _documentRepository.DeleteDocumentAsync(id);
        }
    }
}
