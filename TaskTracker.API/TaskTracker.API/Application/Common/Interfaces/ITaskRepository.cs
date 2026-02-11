using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetAllAsync(TaskItemStatus? status, TaskItemPriority? priority);
    Task<TaskItem> CreateAsync(TaskItem taskItem);
    Task<TaskItem> UpdateAsync(TaskItem taskItem);
}
