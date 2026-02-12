using FluentAssertions;
using NSubstitute;
using TaskTracker.API.Application.Common.Exceptions;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;
using TaskTracker.Test.Common;

namespace TaskTracker.Test.Features.Tasks.Queries;

public class GetTaskByIdQueryHandlerTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly GetTaskByIdQueryHandler _handler;

    public GetTaskByIdQueryHandlerTests()
    {
        MapsterTestConfig.Configure();
        _taskRepository = Substitute.For<ITaskRepository>();
        _handler = new GetTaskByIdQueryHandler(_taskRepository);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsTaskDto()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(id: 1, title: "Found Task", priority: TaskItemPriority.High);
        _taskRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(task);

        var query = new GetTaskByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Found Task");
        result.Priority.Should().Be("High");
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        _taskRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        var query = new GetTaskByIdQuery(999);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*not found*");
    }

    [Fact]
    public async Task Handle_ExistingId_MapsAllFieldsCorrectly()
    {
        // Arrange
        var task = TestDataBuilder.CreateTaskItem(
            id: 5, title: "Full Map", description: "Desc",
            status: TaskItemStatus.InProgress, priority: TaskItemPriority.Low);
        _taskRepository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(task);

        var query = new GetTaskByIdQuery(5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(5);
        result.Title.Should().Be("Full Map");
        result.Description.Should().Be("Desc");
        result.Status.Should().Be("InProgress");
        result.Priority.Should().Be("Low");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
