using Cortex.Mediator.Commands;
using FluentValidation;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<LoginRequest> _validator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IValidator<LoginRequest> validator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new BadRequestException("Validation failed.", errors);
        }

        var user = await _userRepository.GetByEmailAsync(command.Request.Email);
        if (user is null || !_passwordHasher.Verify(command.Request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

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
