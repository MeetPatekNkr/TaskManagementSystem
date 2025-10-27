using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectService _projectService;

        public TaskService(ApplicationDbContext context, IProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task<List<TaskItem>> GetProjectTasksAsync(int projectId, string userId)
        {
            if (!await _projectService.IsUserProjectMemberAsync(projectId, userId))
                throw new UnauthorizedAccessException("You are not a member of this project");

            return await _context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Project)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id, string userId)
        {
            var task = await _context.TaskItems
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Project)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return null;

            if (!await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId))
                throw new UnauthorizedAccessException("You are not a member of this project");

            return task;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskViewModel model, string userId)
        {
            if (!await _projectService.IsUserProjectMemberAsync(model.ProjectId, userId))
                throw new UnauthorizedAccessException("You are not a member of this project");

            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
                Priority = model.Priority,
                ProjectId = model.ProjectId,
                CreatedById = userId,
                AssignedToId = model.AssignedToId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task UpdateTaskAsync(int id, TaskViewModel model, string userId)
        {
            var task = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new ArgumentException("Task not found");

            if (!await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId))
                throw new UnauthorizedAccessException("You are not a member of this project");

            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            task.Status = model.Status;
            task.Priority = model.Priority;
            task.AssignedToId = model.AssignedToId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id, string userId)
        {
            var task = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new ArgumentException("Task not found");

            // Only task creator or project owner/admin can delete
            var isProjectOwnerOrAdmin = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == task.ProjectId && 
                               pm.UserId == userId && 
                               (pm.Role == ProjectRole.Owner || pm.Role == ProjectRole.Admin));

            if (task.CreatedById != userId && !isProjectOwnerOrAdmin)
                throw new UnauthorizedAccessException("You don't have permission to delete this task");

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskStatusAsync(int id, TaskItemStatus status, string userId)
        {
            var task = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new ArgumentException("Task not found");

            if (!await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId))
                throw new UnauthorizedAccessException("You are not a member of this project");

            task.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}