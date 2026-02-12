using Cortex.Mediator.Commands;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Auth.Commands.Register;

public record RegisterCommand(RegisterRequest Request) : ICommand<AuthResponse>;
