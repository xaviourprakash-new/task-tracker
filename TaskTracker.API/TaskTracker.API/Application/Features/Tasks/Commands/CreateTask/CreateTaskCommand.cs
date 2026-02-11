using Cortex.Mediator.Commands;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(CreateTaskRequest Request) : ICommand<TaskItemDto>;
