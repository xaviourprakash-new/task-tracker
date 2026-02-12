using Cortex.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Application.Common.Models;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Tasks.Commands.CreateTask;
using TaskTracker.API.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskTracker.API.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.API.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("tasks")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendCommandAsync<CreateTaskCommand, TaskItemDto>(new CreateTaskCommand(request), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.SuccessResponse(result, "Task created successfully."));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTasks([FromQuery] TaskItemStatus? status, [FromQuery] TaskItemPriority? priority, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendQueryAsync<GetAllTasksQuery, IEnumerable<TaskItemDto>>(new GetAllTasksQuery(status, priority), cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result, "Tasks retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TaskItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendQueryAsync<GetTaskByIdQuery, TaskItemDto?>(new GetTaskByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result, "Task retrieved successfully."));
    }

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<TaskItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendCommandAsync<UpdateTaskStatusCommand, TaskItemDto>(new UpdateTaskStatusCommand(id, request), cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result, "Task status updated successfully."));
    }
}
