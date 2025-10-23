using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class SecureRandomStringGeneratorTests : TestBase
{
    private readonly SecureRandomStringGenerator generator;

    public SecureRandomStringGeneratorTests()
    {
        generator = new SecureRandomStringGenerator();
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        generator.Should().NotBeNull();
    }

    [Fact]
    public void GenerateString_WithValidLength_ShouldReturnStringOfCorrectLength()
    {
        var length = 10;

        var result = generator.GenerateString(length);

        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(length);
    }

    [Fact]
    public void GenerateString_WithLength1_ShouldReturnSingleCharacter()
    {
        var result = generator.GenerateString(1);

        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(1);
    }

    [Fact]
    public void GenerateString_WithLength100_ShouldReturnStringOfLength100()
    {
        var result = generator.GenerateString(100);

        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(100);
    }

    [Fact]
    public void GenerateString_WithZeroLength_ShouldThrowArgumentOutOfRangeException()
    {
        var action = () => generator.GenerateString(0);

        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Length must be a positive integer.*");
    }

    [Fact]
    public void GenerateString_WithNegativeLength_ShouldThrowArgumentOutOfRangeException()
    {
        var action = () => generator.GenerateString(-1);

        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Length must be a positive integer.*");
    }

    [Fact]
    public void GenerateString_ShouldUseValidCharacters()
    {
        var result = generator.GenerateString(1000);

        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex("^[A-Za-z0-9!@#$%^&*()\\-_=+\\[\\]{}|;:,.<>?/]+$");
    }

    [Fact]
    public void GenerateString_ShouldContainOnlyAllowedCharacters()
    {
        var result = generator.GenerateString(1000);
        var allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?/";

        foreach (var character in result)
        {
            allowedCharacters.Should().Contain(character.ToString());
        }
    }

    [Fact]
    public void GenerateString_WithMultipleCalls_ShouldGenerateDifferentStrings()
    {
        var result1 = generator.GenerateString(20);
        var result2 = generator.GenerateString(20);

        result1.Should().NotBe(result2);
    }

    [Fact]
    public void GenerateString_ShouldImplementIRandomValueGenerator()
    {
        generator.Should().BeAssignableTo<IRandomValueGenerator>();
    }

    [Fact]
    public void GenerateString_WithSameLength_ShouldGenerateStringsOfSameLength()
    {
        var length = 15;
        var result1 = generator.GenerateString(length);
        var result2 = generator.GenerateString(length);

        result1.Length.Should().Be(length);
        result2.Length.Should().Be(length);
    }

    [Fact]
    public void GenerateString_ShouldGenerateUniqueStrings()
    {
        var results = new HashSet<string>();
        var iterations = 100;

        for (int i = 0; i < iterations; i++)
        {
            var result = generator.GenerateString(10);
            results.Add(result);
        }

        results.Count.Should().Be(iterations);
    }

    [Fact]
    public void GenerateString_WithLargeLength_ShouldHandleCorrectly()
    {
        var length = 10000;

        var result = generator.GenerateString(length);

        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(length);
    }

    [Fact]
    public void GenerateString_ShouldUseCryptographicallySecureRandom()
    {
        var results = new List<string>();
        var iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            results.Add(generator.GenerateString(20));
        }

        var uniqueResults = results.Distinct().Count();
        uniqueResults.Should().Be(iterations);
    }

    [Fact]
    public void GenerateString_WithRandomLength_ShouldWorkCorrectly()
    {
        var random = new Random();
        var length = random.Next(1, 100);

        var result = generator.GenerateString(length);

        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(length);
    }

    [Fact]
    public void GenerateString_ShouldHandleEdgeCaseLengths()
    {
        var lengths = new[] { 1, 2, 5, 10, 50, 100, 500, 1000 };

        foreach (var length in lengths)
        {
            var result = generator.GenerateString(length);
            result.Should().NotBeNullOrEmpty();
            result.Length.Should().Be(length);
        }
    }

    [Fact]
    public void GenerateString_ShouldNotContainNullCharacters()
    {
        var result = generator.GenerateString(1000);

        result.Should().NotContain('\0'.ToString());
    }

    [Fact]
    public void GenerateString_ShouldNotContainWhitespaceCharacters()
    {
        var result = generator.GenerateString(1000);

        result.Should().NotContain(' '.ToString());
        result.Should().NotContain('\t'.ToString());
        result.Should().NotContain('\n'.ToString());
        result.Should().NotContain('\r'.ToString());
    }

    [Fact]
    public void GenerateString_ShouldGenerateConsistentLength()
    {
        var length = 25;
        var results = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            results.Add(generator.GenerateString(length));
        }

        results.Should().AllSatisfy(result => result.Length.Should().Be(length));
    }

    [Fact]
    public void GenerateString_ShouldUseAllCharacterTypes()
    {
        var result = generator.GenerateString(10000);
        var hasUppercase = result.Any(c => char.IsUpper(c));
        var hasLowercase = result.Any(c => char.IsLower(c));
        var hasDigits = result.Any(c => char.IsDigit(c));
        var hasSpecialChars = result.Any(c => "!@#$%^&*()-_=+[]{}|;:,.<>?/".Contains(c));

        hasUppercase.Should().BeTrue();
        hasLowercase.Should().BeTrue();
        hasDigits.Should().BeTrue();
        hasSpecialChars.Should().BeTrue();
    }
}
