using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.DTOs;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
}
