using FluentAssertions;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Enums;

namespace TaskTracker.Test.Validators;

public class UpdateTaskStatusRequestValidatorTests
{
    private readonly UpdateTaskStatusRequestValidator _validator = new();

    [Theory]
    [InlineData(TaskItemStatus.Pending)]
    [InlineData(TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.Completed)]
    public async Task Validate_ValidStatus_ReturnsValid(TaskItemStatus status)
    {
        var request = new UpdateTaskStatusRequest { Status = status };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_InvalidStatusEnum_ReturnsError()
    {
        var request = new UpdateTaskStatusRequest { Status = (TaskItemStatus)999 };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }
}
