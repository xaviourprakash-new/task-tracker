using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
