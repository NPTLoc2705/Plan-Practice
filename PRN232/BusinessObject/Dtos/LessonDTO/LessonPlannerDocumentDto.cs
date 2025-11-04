using System;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonPlannerDocumentResponse
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int LessonPlannerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LessonPlannerDocumentRequest
    {
        public string FilePath { get; set; }
        public int LessonPlannerId { get; set; }
    }
}
