using FluentAssertions;
using NSubstitute;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;
using TaskTracker.Test.Common;

namespace TaskTracker.Test.Features.Tasks.Queries;

public class GetAllTasksQueryHandlerTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly GetAllTasksQueryHandler _handler;

    public GetAllTasksQueryHandlerTests()
    {
        MapsterTestConfig.Configure();
        _taskRepository = Substitute.For<ITaskRepository>();
        _handler = new GetAllTasksQueryHandler(_taskRepository);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllTasksSortedByPriorityDescending()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            TestDataBuilder.CreateTaskItem(id: 1, title: "Low Task", priority: TaskItemPriority.Low),
            TestDataBuilder.CreateTaskItem(id: 2, title: "High Task", priority: TaskItemPriority.High),
            TestDataBuilder.CreateTaskItem(id: 3, title: "Medium Task", priority: TaskItemPriority.Medium)
        };

        _taskRepository.GetAllAsync(null, null, Arg.Any<CancellationToken>()).Returns(tasks);

        var query = new GetAllTasksQuery(null, null);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Priority.Should().Be("High");
        result[1].Priority.Should().Be("Medium");
        result[2].Priority.Should().Be("Low");
    }

    [Fact]
    public async Task Handle_FilterByStatus_PassesFilterToRepository()
    {
        // Arrange
        _taskRepository.GetAllAsync(TaskItemStatus.Pending, null, Arg.Any<CancellationToken>())
            .Returns(new List<TaskItem>());

        var query = new GetAllTasksQuery(TaskItemStatus.Pending, null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _taskRepository.Received(1).GetAllAsync(TaskItemStatus.Pending, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FilterByPriority_PassesFilterToRepository()
    {
        // Arrange
        _taskRepository.GetAllAsync(null, TaskItemPriority.High, Arg.Any<CancellationToken>())
            .Returns(new List<TaskItem>());

        var query = new GetAllTasksQuery(null, TaskItemPriority.High);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _taskRepository.Received(1).GetAllAsync(null, TaskItemPriority.High, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoTasks_ReturnsEmptyCollection()
    {
        // Arrange
        _taskRepository.GetAllAsync(null, null, Arg.Any<CancellationToken>())
            .Returns(new List<TaskItem>());

        var query = new GetAllTasksQuery(null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_BothFilters_PassesBothToRepository()
    {
        // Arrange
        _taskRepository.GetAllAsync(TaskItemStatus.Pending, TaskItemPriority.High, Arg.Any<CancellationToken>())
            .Returns(new List<TaskItem>());

        var query = new GetAllTasksQuery(TaskItemStatus.Pending, TaskItemPriority.High);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _taskRepository.Received(1).GetAllAsync(
            TaskItemStatus.Pending, TaskItemPriority.High, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SamePriorityTasks_MapsAllFields()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            TestDataBuilder.CreateTaskItem(id: 1, title: "Task A", description: "Desc A", priority: TaskItemPriority.High),
            TestDataBuilder.CreateTaskItem(id: 2, title: "Task B", description: null, priority: TaskItemPriority.High)
        };

        _taskRepository.GetAllAsync(null, null, Arg.Any<CancellationToken>()).Returns(tasks);

        var query = new GetAllTasksQuery(null, null);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Description.Should().Be("Desc A");
        result[1].Description.Should().BeNull();
    }
}
