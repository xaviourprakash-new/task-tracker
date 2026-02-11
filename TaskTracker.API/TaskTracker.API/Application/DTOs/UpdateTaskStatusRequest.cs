using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.DTOs;

public class UpdateTaskStatusRequest
{
    public TaskItemStatus Status { get; set; }
}
