using FluentAssertions;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.Test.Validators;

public class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_ReturnsValid()
    {
        var request = new CreateTaskRequest { Title = "Valid Title", Priority = TaskItemPriority.High };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitle_ReturnsError()
    {
        var request = new CreateTaskRequest { Title = "" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title is required.");
    }

    [Fact]
    public async Task Validate_TitleAt50Characters_ReturnsValid()
    {
        var request = new CreateTaskRequest { Title = new string('A', 50), Priority = TaskItemPriority.Medium };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_TitleAt51Characters_ReturnsError()
    {
        var request = new CreateTaskRequest { Title = new string('A', 51) };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title cannot exceed 50 characters.");
    }

    [Fact]
    public async Task Validate_InvalidPriorityEnum_ReturnsError()
    {
        var request = new CreateTaskRequest { Title = "Valid", Priority = (TaskItemPriority)999 };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Priority");
    }
}
