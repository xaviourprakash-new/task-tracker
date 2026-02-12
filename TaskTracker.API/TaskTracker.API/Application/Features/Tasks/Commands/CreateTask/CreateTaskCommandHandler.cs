using Cortex.Mediator.Commands;
using FluentValidation;
using Mapster;
using TaskTracker.API.Application.Common.Helpers;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IValidator<CreateTaskRequest> _validator;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IValidator<CreateTaskRequest> validator)
    {
        _taskRepository = taskRepository;
        _validator = validator;
    }

    public async Task<TaskItemDto> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        await ValidationHelper.ValidateAndThrowAsync(_validator, command.Request, cancellationToken);

        var taskItem = new TaskItem
        {
            Title = command.Request.Title,
            Description = command.Request.Description,
            Priority = command.Request.Priority,
            Status = TaskItemStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.CreateAsync(taskItem, cancellationToken);

        return taskItem.Adapt<TaskItemDto>();
    }
}
