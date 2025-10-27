using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public InvitationService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Invitation> CreateInvitationAsync(int projectId, string email, string invitedById)
        {
            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new ArgumentException("Project not found");

            var inviter = await _context.Users.FindAsync(invitedById);
            if (inviter == null)
                throw new ArgumentException("Inviter not found");

            // Check for existing pending invitation
            var existingInvitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.ProjectId == projectId && i.Email == email && i.Status == InvitationStatus.Pending);

            if (existingInvitation != null)
                throw new InvalidOperationException("Invitation already sent to this email");

            var token = Guid.NewGuid().ToString();
            var invitation = new Invitation
            {
                Email = email,
                ProjectId = projectId,
                InvitedById = invitedById,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Status = InvitationStatus.Pending
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Send invitation email
            var invitationLink = $"/Invitations/Accept?token={token}";
            await _emailService.SendInvitationEmailAsync(email, project.Name, $"{inviter.FirstName} {inviter.LastName}", invitationLink);

            return invitation;
        }

        public async Task<bool> AcceptInvitationAsync(string token, string userId)
        {
            var invitation = await _context.Invitations
                .Include(i => i.Project)
                .FirstOrDefaultAsync(i => i.Token == token && i.Status == InvitationStatus.Pending);

            if (invitation == null || invitation.ExpiresAt < DateTime.UtcNow)
                return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Email != invitation.Email)
                return false;

            // Add user as project member
            var projectMember = new ProjectMember
            {
                ProjectId = invitation.ProjectId,
                UserId = userId,
                Role = ProjectRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            _context.ProjectMembers.Add(projectMember);

            // Update invitation status
            invitation.Status = InvitationStatus.Accepted;
            invitation.AcceptedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Invitation>> GetPendingInvitationsAsync(string email)
        {
            return await _context.Invitations
                .Where(i => i.Email == email && i.Status == InvitationStatus.Pending && i.ExpiresAt > DateTime.UtcNow)
                .Include(i => i.Project)
                .Include(i => i.InvitedBy)
                .ToListAsync();
        }

        public async Task<bool> IsInvitationValidAsync(string token)
        {
            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Token == token && i.Status == InvitationStatus.Pending && i.ExpiresAt > DateTime.UtcNow);

            return invitation != null;
        }
    }
}