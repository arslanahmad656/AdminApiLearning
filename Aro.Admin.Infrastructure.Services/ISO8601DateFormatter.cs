using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class ISO8601DateFormatter : IDateFormatter
{
    public string Format(DateTimeOffset date) => date.ToString("O");
}
