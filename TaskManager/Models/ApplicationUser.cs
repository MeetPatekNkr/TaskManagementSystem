using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public virtual ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}