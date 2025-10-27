using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        [Required]
        public string CreatedById { get; set; } = string.Empty;

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        public string? AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public virtual ApplicationUser? AssignedTo { get; set; }

        // Navigation properties
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum TaskItemStatus
    {
        ToDo,
        InProgress,
        Review,
        Done
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}