using Cortex.Mediator.Queries;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(int Id) : IQuery<TaskItemDto?>;
