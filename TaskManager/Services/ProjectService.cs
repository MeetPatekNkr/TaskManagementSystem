using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            return await _context.Projects
                .Where(p => p.OwnerId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId))
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .Include(p => p.ProjectMembers)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id, string userId)
        {
            return await _context.Projects
                .Where(p => p.Id == id && (p.OwnerId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId)))
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .FirstOrDefaultAsync();
        }

        public async Task<Project> CreateProjectAsync(ProjectViewModel model, string ownerId)
        {
            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Add owner as project member with Owner role
            var projectMember = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = ownerId,
                Role = ProjectRole.Owner
            };

            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();

            return project;
        }

        public async Task UpdateProjectAsync(int id, ProjectViewModel model, string userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);

            if (project == null)
                throw new UnauthorizedAccessException("You don't have permission to edit this project");

            project.Name = model.Name;
            project.Description = model.Description;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id, string userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);

            if (project == null)
                throw new UnauthorizedAccessException("You don't have permission to delete this project");

            // Delete related data
            var tasks = await _context.TaskItems.Where(t => t.ProjectId == id).ToListAsync();
            var members = await _context.ProjectMembers.Where(pm => pm.ProjectId == id).ToListAsync();
            var invitations = await _context.Invitations.Where(i => i.ProjectId == id).ToListAsync();

            _context.TaskItems.RemoveRange(tasks);
            _context.ProjectMembers.RemoveRange(members);
            _context.Invitations.RemoveRange(invitations);
            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();
        }

        public async Task AddMemberToProjectAsync(int projectId, string email, string currentUserId)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new ArgumentException("Project not found");

            // Check if current user is project owner or admin
            var currentUserRole = project.ProjectMembers
                .FirstOrDefault(pm => pm.UserId == currentUserId)?.Role;

            if (currentUserRole != ProjectRole.Owner && currentUserRole != ProjectRole.Admin)
                throw new UnauthorizedAccessException("You don't have permission to add members");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new ArgumentException("User not found");

            // Check if user is already a member
            if (project.ProjectMembers.Any(pm => pm.UserId == user.Id))
                throw new ArgumentException("User is already a project member");

            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = user.Id,
                Role = ProjectRole.Member
            };

            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();
        }

        public async Task AddMemberDirectlyAsync(int projectId, string userId, string currentUserId)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new ArgumentException("Project not found");

            // Check if current user is project owner or admin
            var currentUserRole = project.ProjectMembers
                .FirstOrDefault(pm => pm.UserId == currentUserId)?.Role;

            if (currentUserRole != ProjectRole.Owner && currentUserRole != ProjectRole.Admin)
                throw new UnauthorizedAccessException("You don't have permission to add members");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Check if user is already a member
            if (project.ProjectMembers.Any(pm => pm.UserId == userId))
                throw new ArgumentException("User is already a project member");

            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                Role = ProjectRole.Member
            };

            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberFromProjectAsync(int projectId, string memberId, string currentUserId)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new ArgumentException("Project not found");

            // Check if current user is project owner or admin
            var currentUserRole = project.ProjectMembers
                .FirstOrDefault(pm => pm.UserId == currentUserId)?.Role;

            if (currentUserRole != ProjectRole.Owner && currentUserRole != ProjectRole.Admin)
                throw new UnauthorizedAccessException("You don't have permission to remove members");

            var member = project.ProjectMembers.FirstOrDefault(pm => pm.UserId == memberId);
            if (member == null)
                throw new ArgumentException("Member not found");

            // Prevent removing owner
            if (member.Role == ProjectRole.Owner)
                throw new InvalidOperationException("Cannot remove project owner");

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserProjectMemberAsync(int projectId, string userId)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }
    }
}