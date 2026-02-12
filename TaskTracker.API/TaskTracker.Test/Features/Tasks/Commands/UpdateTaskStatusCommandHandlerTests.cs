using FluentAssertions;
using NSubstitute;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;
using TaskTracker.Test.Common;

namespace TaskTracker.Test.Features.Tasks.Commands;

public class UpdateTaskStatusCommandHandlerTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly UpdateTaskStatusCommandHandler _handler;

    public UpdateTaskStatusCommandHandlerTests()
    {
        MapsterTestConfig.Configure();
        _taskRepository = Substitute.For<ITaskRepository>();
        var validator = new UpdateTaskStatusRequestValidator();
        _handler = new UpdateTaskStatusCommandHandler(_taskRepository, validator);
    }

    [Fact]
    public async Task Handle_PendingToInProgress_UpdatesSuccessfully()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.Pending);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);
        _taskRepository.UpdateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.InProgress });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("InProgress");
        await _taskRepository.Received(1).UpdateAsync(
            Arg.Is<TaskItem>(t => t.Status == TaskItemStatus.InProgress),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InProgressToCompleted_UpdatesSuccessfully()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.InProgress);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);
        _taskRepository.UpdateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.Completed });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task Handle_PendingToCompleted_ThrowsBadRequestException()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.Pending);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.Completed });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Invalid status transition*Pending*Completed*");
    }

    [Fact]
    public async Task Handle_CompletedTask_ThrowsConflictException()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.Completed);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.InProgress });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Completed tasks cannot be updated*");
    }

    [Fact]
    public async Task Handle_InProgressToPending_ThrowsBadRequestException()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.InProgress);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.Pending });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Invalid status transition*");
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _taskRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        var command = new UpdateTaskStatusCommand(999, new UpdateTaskStatusRequest { Status = TaskItemStatus.InProgress });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*not found*");
    }

    [Fact]
    public async Task Handle_ConflictOrNotFound_DoesNotCallUpdate()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.Completed);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.InProgress });

        // Act
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        await _taskRepository.DidNotReceive().UpdateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PendingToPending_ThrowsBadRequestException()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.Pending);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.Pending });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Invalid status transition*");
    }

    [Fact]
    public async Task Handle_CompletedToCompleted_ThrowsConflictException()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: TaskItemStatus.Completed);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = TaskItemStatus.Completed });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Theory]
    [InlineData(TaskItemStatus.Pending, TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.InProgress, TaskItemStatus.Completed)]
    public async Task Handle_ValidTransitions_ShouldSucceed(TaskItemStatus from, TaskItemStatus to)
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: from);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);
        _taskRepository.UpdateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = to });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(to.ToString());
    }

    [Theory]
    [InlineData(TaskItemStatus.Pending, TaskItemStatus.Completed)]
    [InlineData(TaskItemStatus.InProgress, TaskItemStatus.Pending)]
    public async Task Handle_InvalidTransitions_ShouldThrow(TaskItemStatus from, TaskItemStatus to)
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(status: from);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskStatusCommand(1, new UpdateTaskStatusRequest { Status = to });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
