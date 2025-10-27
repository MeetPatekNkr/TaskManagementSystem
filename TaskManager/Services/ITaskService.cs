using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetProjectTasksAsync(int projectId, string userId);
        Task<TaskItem?> GetTaskByIdAsync(int id, string userId);
        Task<TaskItem> CreateTaskAsync(TaskViewModel model, string userId);
        Task UpdateTaskAsync(int id, TaskViewModel model, string userId);
        Task DeleteTaskAsync(int id, string userId);
        Task UpdateTaskStatusAsync(int id, TaskItemStatus status, string userId);
    }
}