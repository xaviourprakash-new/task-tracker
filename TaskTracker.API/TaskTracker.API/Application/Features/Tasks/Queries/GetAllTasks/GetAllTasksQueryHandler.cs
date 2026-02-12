using Cortex.Mediator.Queries;
using Mapster;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IQueryHandler<GetAllTasksQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllAsync(query.Status, query.Priority, cancellationToken);

        return tasks
            .OrderByDescending(t => t.Priority)
            .Adapt<IEnumerable<TaskItemDto>>();
    }
}
