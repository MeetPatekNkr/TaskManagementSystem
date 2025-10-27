namespace TaskManager.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmailAsync(string email, string projectName, string inviterName, string invitationLink);
    }
}