using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetAllAsync(TaskItemStatus? status, TaskItemPriority? priority, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
}
