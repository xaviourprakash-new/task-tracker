using Cortex.Mediator.Commands;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : ICommandHandler<UpdateTaskStatusCommand, TaskItemDto>
{
    public Task<TaskItemDto> Handle(UpdateTaskStatusCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
