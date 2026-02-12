using FluentAssertions;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.Test.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_ReturnsValid()
    {
        var request = new LoginRequest { Email = "john@example.com", Password = "Pass@123" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyEmail_ReturnsError()
    {
        var request = new LoginRequest { Email = "", Password = "Pass@123" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_EmptyPassword_ReturnsError()
    {
        var request = new LoginRequest { Email = "john@example.com", Password = "" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@missing.domain")]
    [InlineData("no-at-sign")]
    public async Task Validate_InvalidEmailFormats_ReturnsError(string email)
    {
        var request = new LoginRequest { Email = email, Password = "Pass@123" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_BothFieldsEmpty_ReturnsMultipleErrors()
    {
        var request = new LoginRequest { Email = "", Password = "" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
