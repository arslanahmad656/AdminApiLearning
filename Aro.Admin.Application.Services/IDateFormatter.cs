namespace Aro.Admin.Application.Services;

public interface IDateFormatter : IService
{
    string Format(DateTimeOffset date);
}
