using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.DateFormatter;

public interface IDateFormatter : IService
{
    string Format(DateTimeOffset date);
}
