using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using Aro.Common.Application.Services.LogManager;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class ISO8601DateFormatterTests : TestBase
{
    private readonly Mock<ILogManager<ISO8601DateFormatter>> mockLogger;
    private readonly ISO8601DateFormatter formatter;

    public ISO8601DateFormatterTests()
    {
        mockLogger = new Mock<ILogManager<ISO8601DateFormatter>>();
        formatter = new ISO8601DateFormatter(mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        formatter.Should().NotBeNull();
    }

    [Fact]
    public void Format_WithUtcDateTime_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, TimeSpan.Zero);

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.0000000+00:00");
    }

    [Fact]
    public void Format_WithLocalDateTime_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, TimeSpan.FromHours(2));

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.0000000+02:00");
    }

    [Fact]
    public void Format_WithNegativeTimezone_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, TimeSpan.FromHours(-5));

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.0000000-05:00");
    }

    [Fact]
    public void Format_WithMilliseconds_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, 123, TimeSpan.Zero);

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.1230000+00:00");
    }

    [Fact]
    public void Format_WithMicroseconds_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, 123, 456, TimeSpan.Zero);

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.1234560+00:00");
    }

    [Fact]
    public void Format_WithTicks_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, 123, 456, TimeSpan.Zero);

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.1234560+00:00");
    }

    [Fact]
    public void Format_WithMinValue_ShouldReturnIso8601Format()
    {
        var date = DateTimeOffset.MinValue;

        var result = formatter.Format(date);

        result.Should().Be("0001-01-01T00:00:00.0000000+00:00");
    }

    [Fact]
    public void Format_WithMaxValue_ShouldReturnIso8601Format()
    {
        var date = DateTimeOffset.MaxValue;

        var result = formatter.Format(date);

        result.Should().Be("9999-12-31T23:59:59.9999999+00:00");
    }

    [Fact]
    public void Format_WithLeapYear_ShouldReturnIso8601Format()
    {
        var date = new DateTimeOffset(2024, 2, 29, 12, 0, 0, TimeSpan.Zero);

        var result = formatter.Format(date);

        result.Should().Be("2024-02-29T12:00:00.0000000+00:00");
    }

    [Fact]
    public void Format_WithDifferentTimezones_ShouldReturnCorrectFormat()
    {
        var utcDate = new DateTimeOffset(2024, 1, 15, 12, 0, 0, TimeSpan.Zero);
        var estDate = new DateTimeOffset(2024, 1, 15, 7, 0, 0, TimeSpan.FromHours(-5));
        var pstDate = new DateTimeOffset(2024, 1, 15, 4, 0, 0, TimeSpan.FromHours(-8));

        var utcResult = formatter.Format(utcDate);
        var estResult = formatter.Format(estDate);
        var pstResult = formatter.Format(pstDate);

        utcResult.Should().Be("2024-01-15T12:00:00.0000000+00:00");
        estResult.Should().Be("2024-01-15T07:00:00.0000000-05:00");
        pstResult.Should().Be("2024-01-15T04:00:00.0000000-08:00");
    }

    [Fact]
    public void Format_ShouldLogDebugMessages()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, TimeSpan.Zero);

        formatter.Format(date);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Format"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Date formatted successfully: {FormattedDate}", "2024-01-15T14:30:45.0000000+00:00"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Format"), Times.Once);
    }

    [Fact]
    public void Format_WithRandomDate_ShouldWorkCorrectly()
    {
        var random = new Random();
        var daysOffset = random.Next(-365, 365);
        var date = DateTimeOffset.UtcNow.AddDays(daysOffset);

        var result = formatter.Format(date);

        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{7}[+-]\d{2}:\d{2}$");
    }

    [Fact]
    public void Format_WithCurrentDateTime_ShouldReturnValidIso8601()
    {
        var date = DateTimeOffset.UtcNow;

        var result = formatter.Format(date);

        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{7}[+-]\d{2}:\d{2}$");
        
        var parsedDate = DateTimeOffset.Parse(result);
        parsedDate.Should().BeCloseTo(date, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void Format_WithSpecificTimezone_ShouldPreserveTimezone()
    {
        var timezone = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(30));
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, timezone);

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.0000000+05:30");
    }

    [Fact]
    public void Format_WithFractionalSeconds_ShouldHandleCorrectly()
    {
        var date = new DateTimeOffset(2024, 1, 15, 14, 30, 45, 500, TimeSpan.Zero);

        var result = formatter.Format(date);

        result.Should().Be("2024-01-15T14:30:45.5000000+00:00");
    }
}
