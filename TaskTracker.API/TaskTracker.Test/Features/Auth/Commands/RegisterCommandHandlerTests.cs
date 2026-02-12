using FluentAssertions;
using NSubstitute;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Auth.Commands.Register;
using TaskTracker.API.Domain.Entities;

namespace TaskTracker.Test.Features.Auth.Commands;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        var validator = new RegisterRequestValidator();

        _handler = new RegisterCommandHandler(_userRepository, _jwtTokenGenerator, _passwordHasher, validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Jane Doe",
            Email = "jane@example.com",
            Password = "Pass@123"
        };

        _userRepository.GetByEmailAsync("jane@example.com").Returns((User?)null);
        _passwordHasher.Hash("Pass@123").Returns("hashed_password");
        _userRepository.CreateAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());
        _jwtTokenGenerator.GenerateToken(Arg.Any<User>())
            .Returns(("jwt_token_here", DateTime.UtcNow.AddHours(1)));

        var command = new RegisterCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_token_here");
        result.Email.Should().Be("jane@example.com");
        result.FullName.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task Handle_ValidRequest_HashesPasswordBeforeStoring()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Jane Doe",
            Email = "jane@example.com",
            Password = "Pass@123"
        };

        _userRepository.GetByEmailAsync("jane@example.com").Returns((User?)null);
        _passwordHasher.Hash("Pass@123").Returns("securely_hashed");
        _userRepository.CreateAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());
        _jwtTokenGenerator.GenerateToken(Arg.Any<User>())
            .Returns(("token", DateTime.UtcNow.AddHours(1)));

        var command = new RegisterCommand(request);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasher.Received(1).Hash("Pass@123");
        await _userRepository.Received(1).CreateAsync(
            Arg.Is<User>(u => u.PasswordHash == "securely_hashed"));
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsConflictException()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "john@example.com" };
        _userRepository.GetByEmailAsync("john@example.com").Returns(existingUser);

        var request = new RegisterRequest
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "Pass@123"
        };
        var command = new RegisterCommand(request);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*john@example.com*already exists*");
    }

    [Fact]
    public async Task Handle_EmptyFields_ThrowsBadRequestException()
    {
        // Arrange
        var request = new RegisterRequest { FullName = "", Email = "", Password = "" };
        var command = new RegisterCommand(request);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("FullName");
        exception.Which.Errors.Should().ContainKey("Email");
        exception.Which.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task Handle_ShortPassword_ThrowsBadRequestException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Jane",
            Email = "jane@example.com",
            Password = "12"
        };
        var command = new RegisterCommand(request);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task Handle_PasswordAtMinLength_Succeeds()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Jane",
            Email = "jane@example.com",
            Password = "123456"
        };

        _userRepository.GetByEmailAsync("jane@example.com").Returns((User?)null);
        _passwordHasher.Hash("123456").Returns("hashed");
        _userRepository.CreateAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());
        _jwtTokenGenerator.GenerateToken(Arg.Any<User>()).Returns(("token", DateTime.UtcNow.AddHours(1)));

        var command = new RegisterCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_ThrowsBadRequestException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Jane",
            Email = "not-an-email",
            Password = "Pass@123"
        };
        var command = new RegisterCommand(request);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_DoesNotHashPassword()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "dup@example.com" };
        _userRepository.GetByEmailAsync("dup@example.com").Returns(existingUser);

        var request = new RegisterRequest
        {
            FullName = "Jane",
            Email = "dup@example.com",
            Password = "Pass@123"
        };
        var command = new RegisterCommand(request);

        // Act
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        _passwordHasher.DidNotReceive().Hash(Arg.Any<string>());
    }
}
