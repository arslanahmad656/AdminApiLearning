using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class BCryptHasherTests : TestBase
{
    private readonly Mock<ILogManager<BCryptHasher>> mockLogger;
    private readonly BCryptHasher hasher;

    public BCryptHasherTests()
    {
        mockLogger = new Mock<ILogManager<BCryptHasher>>();
        hasher = new BCryptHasher(mockLogger.Object);
    }

    [Fact]
    public void Hash_ShouldReturnHashedString()
    {
        var password = fixture.Create<string>();

        var result = hasher.Hash(password);

        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe(password);
        result.Should().StartWith("$2");
        result.Length.Should().BeGreaterThan(50);
    }

    [Fact]
    public void Hash_ShouldReturnDifferentHashesForSameInput()
    {
        var password = fixture.Create<string>();

        var hash1 = hasher.Hash(password);
        var hash2 = hasher.Hash(password);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Hash_ShouldLogDebugMessages()
    {
        var password = fixture.Create<string>();

        hasher.Hash(password);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Hash"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Password hashed successfully"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Hash"), Times.Once);
    }

    [Fact]
    public void Verify_WithValidPassword_ShouldReturnTrue()
    {
        var password = fixture.Create<string>();
        var hash = hasher.Hash(password);

        var result = hasher.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WithInvalidPassword_ShouldReturnFalse()
    {
        var password = fixture.Create<string>();
        var wrongPassword = fixture.Create<string>();
        var hash = hasher.Hash(password);

        var result = hasher.Verify(wrongPassword, hash);

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_WithInvalidHash_ShouldThrowException()
    {
        var password = fixture.Create<string>();
        var invalidHash = "invalid_hash_format";

        Action act = () => hasher.Verify(password, invalidHash);

        act.Should().Throw<BCrypt.Net.SaltParseException>();
    }

    [Fact]
    public void Verify_ShouldLogDebugMessages()
    {
        var password = fixture.Create<string>();
        var hash = hasher.Hash(password);

        hasher.Verify(password, hash);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Verify"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Password verification completed, isValid: {IsValid}", It.IsAny<bool>()), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Verify"), Times.Once);
    }

    [Fact]
    public void HashAndVerify_ShouldWorkWithVariousPasswords()
    {
        var passwords = new[]
        {
            "simple",
            "complex!@#$%^&*()",
            "1234567890",
            "a",
            "verylongpasswordthatexceedsnormallimitsandshouldstillworkcorrectly",
            "password with spaces",
            "PASSWORD",
            "Password123",
            "特殊字符测试",
            ""
        };

        foreach (var password in passwords)
        {
            var hash = hasher.Hash(password);
            var isValid = hasher.Verify(password, hash);
            
            isValid.Should().BeTrue($"Password '{password}' should hash and verify correctly");
        }
    }

    [Fact]
    public void Hash_ShouldHandleNullInput()
    {
        Action act = () => hasher.Hash(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Verify_ShouldHandleNullPassword()
    {
        var hash = fixture.Create<string>();

        Action act = () => hasher.Verify(null!, hash);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Verify_ShouldHandleNullHash()
    {
        var password = fixture.Create<string>();

        Action act = () => hasher.Verify(password, null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
