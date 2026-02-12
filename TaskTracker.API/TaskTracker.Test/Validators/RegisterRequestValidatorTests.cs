using FluentAssertions;
using TaskTracker.API.Application.Common.Validators;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.Test.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_ReturnsValid()
    {
        var request = new RegisterRequest
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "Pass@123"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_AllFieldsEmpty_ReturnsAllErrors()
    {
        var request = new RegisterRequest { FullName = "", Email = "", Password = "" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    [InlineData("@nodomain")]
    public async Task Validate_InvalidEmailFormats_ReturnsError(string email)
    {
        var request = new RegisterRequest { FullName = "Jane", Email = email, Password = "Pass@123" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_PasswordAt5Characters_ReturnsError()
    {
        var request = new RegisterRequest { FullName = "Jane", Email = "jane@test.com", Password = "12345" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Validate_PasswordAt6Characters_ReturnsValid()
    {
        var request = new RegisterRequest { FullName = "Jane", Email = "jane@test.com", Password = "123456" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_FullNameAt100Characters_ReturnsValid()
    {
        var request = new RegisterRequest
        {
            FullName = new string('A', 100),
            Email = "jane@test.com",
            Password = "Pass@123"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_FullNameAt101Characters_ReturnsError()
    {
        var request = new RegisterRequest
        {
            FullName = new string('A', 101),
            Email = "jane@test.com",
            Password = "Pass@123"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public async Task Validate_PasswordAt100Characters_ReturnsValid()
    {
        var request = new RegisterRequest
        {
            FullName = "Jane",
            Email = "jane@test.com",
            Password = new string('A', 100)
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_PasswordAt101Characters_ReturnsError()
    {
        var request = new RegisterRequest
        {
            FullName = "Jane",
            Email = "jane@test.com",
            Password = new string('A', 101)
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
