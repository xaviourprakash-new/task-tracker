using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Application.Common.Models;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Auth.Commands.Login;
using TaskTracker.API.Application.Features.Auth.Commands.Register;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendCommandAsync<RegisterCommand, AuthResponse>(new RegisterCommand(request), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.SuccessResponse(result, "User registered successfully."));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendCommandAsync<LoginCommand, AuthResponse>(new LoginCommand(request), cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result, "Login successful."));
    }
}
