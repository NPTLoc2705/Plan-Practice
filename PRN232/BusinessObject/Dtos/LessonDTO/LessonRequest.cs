using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Grade level is required")]
        [Range(1, 12, ErrorMessage = "Grade level must be between 1 and 12")]
        public int GradeLevel { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        // UserId is removed - will be set from JWT token automatically
    }
}