using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IInvitationService _invitationService;
        private readonly ApplicationDbContext _context;

        public ProjectsController(IProjectService projectService, IInvitationService invitationService, ApplicationDbContext context)
        {
            _projectService = projectService;
            _invitationService = invitationService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var projects = await _projectService.GetUserProjectsAsync(userId);
            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                await _projectService.CreateProjectAsync(model, userId);
                TempData["SuccessMessage"] = "Project created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating project: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var project = await _projectService.GetProjectByIdAsync(id, userId);
                if (project == null)
                    return NotFound();

                var members = project.ProjectMembers.Select(pm => new ProjectMemberViewModel
                {
                    UserId = pm.UserId,
                    UserName = $"{pm.User.FirstName} {pm.User.LastName}",
                    Email = pm.User.Email ?? string.Empty,
                    Role = pm.Role.ToString(),
                    JoinedAt = pm.JoinedAt
                }).ToList();

                // Calculate counts safely
                int adminCount = 0;
                int regularMemberCount = 0;
                
                foreach (var member in members)
                {
                    if (member.Role == "Owner" || member.Role == "Admin")
                    {
                        adminCount++;
                    }
                    else if (member.Role == "Member")
                    {
                        regularMemberCount++;
                    }
                }

                var viewModel = new ProjectDetailViewModel
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    OwnerName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    TaskCount = project.Tasks.Count,
                    MemberCount = project.ProjectMembers.Count,
                    AdminCount = adminCount,
                    RegularMemberCount = regularMemberCount,
                    Members = members
                };

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                await _projectService.DeleteProjectAsync(id, userId);
                TempData["SuccessMessage"] = "Project deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting project: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendInvitation(int projectId, string email)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                await _invitationService.CreateInvitationAsync(projectId, email, userId);
                return Json(new { success = true, message = "Invitation sent successfully! The user will receive an email to join the project." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMemberDirectly(int projectId, string userId)
        {
            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                    return Json(new { success = false, message = "User not authenticated" });

                await _projectService.AddMemberDirectlyAsync(projectId, userId, currentUserId);
                return Json(new { success = true, message = "Member added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember(int projectId, string memberId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                await _projectService.RemoveMemberFromProjectAsync(projectId, memberId, userId);
                return Json(new { success = true, message = "Member removed successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 2)
                    return Json(new List<object>());

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var users = await _context.Users
                    .Where(u => (u.Email != null && u.Email.Contains(searchTerm)) || 
                                (u.FirstName != null && u.FirstName.Contains(searchTerm)) || 
                                (u.LastName != null && u.LastName.Contains(searchTerm)) &&
                                u.Id != currentUserId) // Exclude current user
                    .Select(u => new
                    {
                        id = u.Id,
                        email = u.Email ?? string.Empty,
                        fullName = (u.FirstName + " " + u.LastName).Trim()
                    })
                    .Take(10)
                    .ToListAsync();

                return Json(users);
            }
            catch (Exception)
            {
                // Return empty list to avoid breaking the UI
                return Json(new List<object>());
            }
        }
    }
}