using Cortex.Mediator.Commands;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(int Id, UpdateTaskStatusRequest Request) : ICommand<TaskItemDto>;
