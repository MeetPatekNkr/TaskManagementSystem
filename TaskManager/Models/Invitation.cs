using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Invitation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        [Required]
        public string InvitedById { get; set; } = string.Empty;

        [ForeignKey("InvitedById")]
        public virtual ApplicationUser InvitedBy { get; set; } = null!;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        [Required]
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

        public DateTime? AcceptedAt { get; set; }
    }

    public enum InvitationStatus
    {
        Pending,
        Accepted,
        Expired,
        Cancelled
    }
}