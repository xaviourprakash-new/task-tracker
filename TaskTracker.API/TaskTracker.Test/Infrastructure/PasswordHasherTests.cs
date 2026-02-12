using FluentAssertions;
using TaskTracker.API.Infrastructure.Auth;

namespace TaskTracker.Test.Infrastructure;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        var result = _hasher.Hash("password");

        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Hash_ContainsSaltAndHash()
    {
        var result = _hasher.Hash("password");

        result.Should().Contain(":");
        var parts = result.Split(':');
        parts.Should().HaveCount(2);
        parts[0].Should().NotBeEmpty();
        parts[1].Should().NotBeEmpty();
    }

    [Fact]
    public void Hash_SamePasswordProducesDifferentHashes()
    {
        var hash1 = _hasher.Hash("password");
        var hash2 = _hasher.Hash("password");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.Hash("MySecret123");

        var result = _hasher.Verify("MySecret123", hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("MySecret123");

        var result = _hasher.Verify("WrongPassword", hash);

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_TamperedHash_ReturnsFalse()
    {
        var hash = _hasher.Hash("password");
        var tampered = hash + "tampered";

        var result = _hasher.Verify("password", tampered);

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_InvalidFormat_ReturnsFalse()
    {
        var result = _hasher.Verify("password", "no-colon-here");

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_EmptyString_ReturnsFalse()
    {
        var result = _hasher.Verify("password", "");

        result.Should().BeFalse();
    }
}
