using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetUserProjectsAsync(string userId);
        Task<Project?> GetProjectByIdAsync(int id, string userId);
        Task<Project> CreateProjectAsync(ProjectViewModel model, string ownerId);
        Task UpdateProjectAsync(int id, ProjectViewModel model, string userId);
        Task DeleteProjectAsync(int id, string userId);
        Task AddMemberToProjectAsync(int projectId, string email, string currentUserId);
        Task AddMemberDirectlyAsync(int projectId, string userId, string currentUserId);
        Task RemoveMemberFromProjectAsync(int projectId, string memberId, string currentUserId);
        Task<bool> IsUserProjectMemberAsync(int projectId, string userId);
    }
}