using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
        [Display(Name = "Project Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }

    public class ProjectDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public int MemberCount { get; set; }
        
        // Team statistics
        public int AdminCount { get; set; }
        public int RegularMemberCount { get; set; }
        
        public List<ProjectMemberViewModel> Members { get; set; } = new();
    }

    public class ProjectMemberViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}