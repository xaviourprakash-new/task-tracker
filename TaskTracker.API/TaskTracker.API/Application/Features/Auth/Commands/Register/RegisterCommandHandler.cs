using Cortex.Mediator.Commands;
using FluentValidation;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Helpers;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Entities;

namespace TaskTracker.API.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterRequest> _validator;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IValidator<RegisterRequest> validator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<AuthResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        await ValidationHelper.ValidateAndThrowAsync(_validator, command.Request, cancellationToken);

        var existingUser = await _userRepository.GetByEmailAsync(command.Request.Email);
        if (existingUser is not null)
            throw new ConflictException($"User with email '{command.Request.Email}' already exists.");

        var user = new User
        {
            FullName = command.Request.FullName,
            Email = command.Request.Email,
            PasswordHash = _passwordHasher.Hash(command.Request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            ExpiresAt = expiresAt
        };
    }
}
