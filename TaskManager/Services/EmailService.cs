using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace TaskManager.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpSettings _smtpSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(ILogger<EmailService> logger, 
                           IOptions<SmtpSettings> smtpSettings, 
                           IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendInvitationEmailAsync(string email, string projectName, string inviterName, string invitationLink)
        {
            try
            {
                // For development, log the email instead of sending
                _logger.LogInformation("=== EMAIL INVITATION ===");
                _logger.LogInformation("To: {Email}", email);
                _logger.LogInformation("Project: {ProjectName}", projectName);
                _logger.LogInformation("Inviter: {InviterName}", inviterName);
                _logger.LogInformation("Invitation Link: {InvitationLink}", invitationLink);
                _logger.LogInformation("=== END EMAIL ===");

                // Simulate email sending delay
                await Task.Delay(100);

                _logger.LogInformation("Invitation email processed for: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invitation email to: {Email}", email);
                throw new Exception($"Failed to process invitation: {ex.Message}");
            }
        }
    }

    public class SmtpSettings
    {
        public string Server { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}