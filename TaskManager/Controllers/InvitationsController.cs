using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    public class InvitationsController : Controller
    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [Authorize]
        public async Task<IActionResult> Accept(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Invalid invitation token.";
                return View("Error");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var success = await _invitationService.AcceptInvitationAsync(token, userId);
            
            if (success)
            {
                ViewBag.SuccessMessage = "You have successfully joined the project!";
                return View("InvitationAccepted");
            }
            else
            {
                ViewBag.ErrorMessage = "This invitation is invalid or has expired.";
                return View("InvalidInvitation");
            }
        }
    }
}