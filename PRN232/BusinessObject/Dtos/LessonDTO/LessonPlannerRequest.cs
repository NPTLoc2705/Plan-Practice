using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Dtos.LessonDTO
{
    public class LessonPlannerRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Class ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Class ID")]
        public int ClassId { get; set; }
    }
}