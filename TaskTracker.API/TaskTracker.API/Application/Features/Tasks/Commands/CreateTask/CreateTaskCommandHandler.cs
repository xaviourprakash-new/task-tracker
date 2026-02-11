using Cortex.Mediator.Commands;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, TaskItemDto>
{
    public Task<TaskItemDto> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
