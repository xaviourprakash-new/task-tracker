using Cortex.Mediator.Queries;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, TaskItemDto?>
{
    public Task<TaskItemDto?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
