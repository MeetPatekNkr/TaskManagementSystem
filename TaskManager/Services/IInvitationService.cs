using TaskManager.Models;

namespace TaskManager.Services
{
    public interface IInvitationService
    {
        Task<Invitation> CreateInvitationAsync(int projectId, string email, string invitedById);
        Task<bool> AcceptInvitationAsync(string token, string userId);
        Task<List<Invitation>> GetPendingInvitationsAsync(string email);
        Task<bool> IsInvitationValidAsync(string token);
    }
}