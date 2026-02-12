using FluentAssertions;
using NSubstitute;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Auth.Commands.Login;
using TaskTracker.API.Domain.Entities;
using TaskTracker.Test.Common;

namespace TaskTracker.Test.Features.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        var validator = new LoginRequestValidator();

        _handler = new LoginCommandHandler(_userRepository, _jwtTokenGenerator, _passwordHasher, validator);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(passwordHash: "stored_hash");
        _userRepository.GetByEmailAsync("john@example.com").Returns(user);
        _passwordHasher.Verify("Pass@123", "stored_hash").Returns(true);
        _jwtTokenGenerator.GenerateToken(user).Returns(("jwt_token", DateTime.UtcNow.AddHours(1)));

        var command = new LoginCommand(new LoginRequest { Email = "john@example.com", Password = "Pass@123" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_token");
        result.Email.Should().Be("john@example.com");
        result.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        _userRepository.GetByEmailAsync("unknown@example.com").Returns((User?)null);

        var command = new LoginCommand(new LoginRequest { Email = "unknown@example.com", Password = "Pass@123" });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("*Invalid email or password*");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(passwordHash: "stored_hash");
        _userRepository.GetByEmailAsync("john@example.com").Returns(user);
        _passwordHasher.Verify("WrongPassword", "stored_hash").Returns(false);

        var command = new LoginCommand(new LoginRequest { Email = "john@example.com", Password = "WrongPassword" });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("*Invalid email or password*");
    }

    [Fact]
    public async Task Handle_WrongPassword_DoesNotGenerateToken()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(passwordHash: "stored_hash");
        _userRepository.GetByEmailAsync("john@example.com").Returns(user);
        _passwordHasher.Verify("WrongPassword", "stored_hash").Returns(false);

        var command = new LoginCommand(new LoginRequest { Email = "john@example.com", Password = "WrongPassword" });

        // Act
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        _jwtTokenGenerator.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_EmptyEmail_ThrowsBadRequestException()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Email = "", Password = "Pass@123" });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Handle_EmptyPassword_ThrowsBadRequestException()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Email = "john@example.com", Password = "" });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_ThrowsBadRequestException()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Email = "not-an-email", Password = "Pass@123" });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Handle_ValidationFails_DoesNotCallRepository()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Email = "", Password = "" });

        // Act
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>());
    }
}
