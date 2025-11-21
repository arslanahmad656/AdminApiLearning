using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.DateFormatter;

public interface IDateFormatter : IService
{
    string Format(DateTimeOffset date);
}
