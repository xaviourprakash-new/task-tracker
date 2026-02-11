using Cortex.Mediator.Queries;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.Features.Tasks.Queries.GetAllTasks;

public record GetAllTasksQuery(TaskItemStatus? Status, TaskItemPriority? Priority) : IQuery<IEnumerable<TaskItemDto>>;
