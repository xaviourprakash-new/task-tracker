using Cortex.Mediator.Queries;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IQueryHandler<GetAllTasksQuery, IEnumerable<TaskItemDto>>
{
    public Task<IEnumerable<TaskItemDto>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
