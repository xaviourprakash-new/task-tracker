using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Tasks.Commands.CreateTask;
using TaskTracker.API.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskTracker.API.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.API.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] TaskItemStatus? status, [FromQuery] TaskItemPriority? priority)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusRequest request)
    {
        throw new NotImplementedException();
    }
}
