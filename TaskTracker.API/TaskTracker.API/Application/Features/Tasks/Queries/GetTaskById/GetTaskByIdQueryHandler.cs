using Cortex.Mediator.Queries;
using Mapster;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, TaskItemDto?>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskItemDto?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        var taskItem = await _taskRepository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), query.Id);

        return taskItem.Adapt<TaskItemDto>();
    }
}
