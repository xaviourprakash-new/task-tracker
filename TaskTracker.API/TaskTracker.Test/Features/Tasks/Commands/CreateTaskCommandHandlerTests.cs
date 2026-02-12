using FluentAssertions;
using NSubstitute;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Tasks.Commands.CreateTask;
using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;
using TaskTracker.Test.Common;

namespace TaskTracker.Test.Features.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        MapsterTestConfig.Configure();
        _taskRepository = Substitute.For<ITaskRepository>();
        var validator = new CreateTaskRequestValidator();
        _handler = new CreateTaskCommandHandler(_taskRepository, validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesTaskWithPendingStatus()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Task Description",
            Priority = TaskItemPriority.High
        };
        var command = new CreateTaskCommand(request);

        _taskRepository.CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TaskItem>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Status.Should().Be("Pending");
        result.Priority.Should().Be("High");

        await _taskRepository.Received(1).CreateAsync(
            Arg.Is<TaskItem>(t =>
                t.Title == "New Task" &&
                t.Status == TaskItemStatus.Pending &&
                t.Priority == TaskItemPriority.High),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidRequest_SetsCreatedAtToUtc()
    {
        // Arrange
        var beforeUtc = DateTime.UtcNow;
        var request = new CreateTaskRequest { Title = "UTC Test", Priority = TaskItemPriority.Low };
        var command = new CreateTaskCommand(request);

        _taskRepository.CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TaskItem>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeUtc);
        result.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public async Task Handle_EmptyTitle_ThrowsBadRequestException()
    {
        // Arrange
        var request = new CreateTaskRequest { Title = "", Priority = TaskItemPriority.Medium };
        var command = new CreateTaskCommand(request);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Title");
    }

    [Fact]
    public async Task Handle_TitleExceeds50Characters_ThrowsBadRequestException()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = new string('A', 51),
            Priority = TaskItemPriority.Medium
        };
        var command = new CreateTaskCommand(request);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BadRequestException>();
        exception.Which.Errors.Should().ContainKey("Title");
    }

    [Fact]
    public async Task Handle_ValidationFails_DoesNotCallRepository()
    {
        // Arrange
        var request = new CreateTaskRequest { Title = "", Priority = TaskItemPriority.Medium };
        var command = new CreateTaskCommand(request);

        // Act
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        await _taskRepository.DidNotReceive().CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullDescription_CreatesTaskSuccessfully()
    {
        // Arrange
        var request = new CreateTaskRequest { Title = "No Desc", Description = null, Priority = TaskItemPriority.Low };
        var command = new CreateTaskCommand(request);

        _taskRepository.CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TaskItem>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(TaskItemPriority.Low)]
    [InlineData(TaskItemPriority.Medium)]
    [InlineData(TaskItemPriority.High)]
    public async Task Handle_AllPriorities_MapsCorrectly(TaskItemPriority priority)
    {
        // Arrange
        var request = new CreateTaskRequest { Title = "Priority Test", Priority = priority };
        var command = new CreateTaskCommand(request);

        _taskRepository.CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TaskItem>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Priority.Should().Be(priority.ToString());
    }

    [Fact]
    public async Task Handle_CancellationRequested_PropagatesCancellationToken()
    {
        // Arrange
        var request = new CreateTaskRequest { Title = "Cancel Test", Priority = TaskItemPriority.Medium };
        var command = new CreateTaskCommand(request);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _taskRepository.CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<CancellationToken>().ThrowIfCancellationRequested();
                return callInfo.Arg<TaskItem>();
            });

        // Act
        var act = () => _handler.Handle(command, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
