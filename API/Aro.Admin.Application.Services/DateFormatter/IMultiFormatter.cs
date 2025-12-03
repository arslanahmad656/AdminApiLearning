using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.DateFormatter;

public interface IMultiFormatter : IService
{
    string Format(DateTimeOffset date);

    string FormatTime(TimeOnly time);

    string FormatDate(DateOnly date);

    string FormatGuidCompact(Guid guid);
}
