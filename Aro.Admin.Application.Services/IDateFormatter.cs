namespace Aro.Admin.Application.Services;

public interface IDateFormatter
{
    string Format(DateTimeOffset date);
}
