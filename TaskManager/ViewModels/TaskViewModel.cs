using System.ComponentModel.DataAnnotations;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(200, ErrorMessage = "Task title cannot exceed 200 characters")]
        [Display(Name = "Task Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public TaskItemStatus Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public TaskPriority Priority { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Assign To")]
        public string? AssignedToId { get; set; }
        
        public string? AssignedToName { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }

    public class TaskDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new();
    }
}