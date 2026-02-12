using Cortex.Mediator.Commands;
using FluentValidation;
using Mapster;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Helpers;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : ICommandHandler<UpdateTaskStatusCommand, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IValidator<UpdateTaskStatusRequest> _validator;

    public UpdateTaskStatusCommandHandler(ITaskRepository taskRepository, IValidator<UpdateTaskStatusRequest> validator)
    {
        _taskRepository = taskRepository;
        _validator = validator;
    }

    public async Task<TaskItemDto> Handle(UpdateTaskStatusCommand command, CancellationToken cancellationToken)
    {
        await ValidationHelper.ValidateAndThrowAsync(_validator, command.Request, cancellationToken);

        var taskItem = await _taskRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), command.Id);

        if (taskItem.Status == TaskItemStatus.Completed)
            throw new ConflictException("Completed tasks cannot be updated.");

        var isValidTransition = (taskItem.Status, command.Request.Status) switch
        {
            (TaskItemStatus.Pending, TaskItemStatus.InProgress) => true,
            (TaskItemStatus.InProgress, TaskItemStatus.Completed) => true,
            _ => false
        };

        if (!isValidTransition)
            throw new BadRequestException(
                $"Invalid status transition from '{taskItem.Status}' to '{command.Request.Status}'. " +
                $"Allowed transitions: Pending → InProgress → Completed.");

        taskItem.Status = command.Request.Status;
        await _taskRepository.UpdateAsync(taskItem, cancellationToken);

        return taskItem.Adapt<TaskItemDto>();
    }
}
