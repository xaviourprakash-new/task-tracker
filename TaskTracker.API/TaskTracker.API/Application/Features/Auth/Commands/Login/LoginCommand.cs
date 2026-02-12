using Cortex.Mediator.Commands;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginRequest Request) : ICommand<AuthResponse>;
