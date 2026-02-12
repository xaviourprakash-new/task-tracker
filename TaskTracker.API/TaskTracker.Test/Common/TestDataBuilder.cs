using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.Test.Common;

public static class TestDataBuilder
{
    public static TaskItem CreateTaskItem(
        int id = 1,
        string title = "Test Task",
        string? description = "Test Description",
        TaskItemStatus status = TaskItemStatus.Pending,
        TaskItemPriority priority = TaskItemPriority.Medium)
    {
        return new TaskItem
        {
            Id = id,
            Title = title,
            Description = description,
            Status = status,
            Priority = priority,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User CreateUser(
        int id = 1,
        string fullName = "John Doe",
        string email = "john@example.com",
        string passwordHash = "hashedpassword")
    {
        return new User
        {
            Id = id,
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }
}
