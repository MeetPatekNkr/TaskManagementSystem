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
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ApplicationDbContext _context;

        public TasksController(ITaskService taskService, ApplicationDbContext context)
        {
            _taskService = taskService;
            _context = context;
        }

        public async Task<IActionResult> Index(int projectId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var tasks = await _taskService.GetProjectTasksAsync(projectId, userId);
                ViewBag.ProjectId = projectId;
                ViewBag.ProjectName = await _context.Projects
                    .Where(p => p.Id == projectId)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                return View(tasks);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        public async Task<IActionResult> Create(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return NotFound();

            ViewBag.Project = project;
            ViewBag.Users = project.ProjectMembers.Select(pm => pm.User).ToList();

            var model = new TaskViewModel
            {
                ProjectId = projectId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var project = await _context.Projects
                    .Include(p => p.ProjectMembers)
                        .ThenInclude(pm => pm.User)
                    .FirstOrDefaultAsync(p => p.Id == model.ProjectId);

                if (project != null)
                {
                    ViewBag.Project = project;
                    ViewBag.Users = project.ProjectMembers.Select(pm => pm.User).ToList();
                }
                return View(model);
            }

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                await _taskService.CreateTaskAsync(model, userId);
                return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating task: {ex.Message}");
                
                var project = await _context.Projects
                    .Include(p => p.ProjectMembers)
                        .ThenInclude(pm => pm.User)
                    .FirstOrDefaultAsync(p => p.Id == model.ProjectId);

                if (project != null)
                {
                    ViewBag.Project = project;
                    ViewBag.Users = project.ProjectMembers.Select(pm => pm.User).ToList();
                }
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

                var task = await _taskService.GetTaskByIdAsync(id, userId);
                if (task == null)
                    return NotFound();

                var viewModel = new TaskDetailViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    CreatedAt = task.CreatedAt,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    CreatedByName = $"{task.CreatedBy.FirstName} {task.CreatedBy.LastName}",
                    AssignedToName = task.AssignedTo != null ? 
                        $"{task.AssignedTo.FirstName} {task.AssignedTo.LastName}" : "Unassigned",
                    ProjectName = task.Project.Name,
                    ProjectId = task.ProjectId,
                    Comments = task.Comments.Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UserName = $"{c.User.FirstName} {c.User.LastName}",
                        TaskItemId = c.TaskItemId
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, TaskItemStatus status)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                await _taskService.UpdateTaskStatusAsync(id, status, userId);
                return Json(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}