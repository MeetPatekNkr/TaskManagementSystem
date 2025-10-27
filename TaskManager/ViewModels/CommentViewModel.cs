using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment content is required")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Display(Name = "Comment")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TaskItemId { get; set; }
    }
}