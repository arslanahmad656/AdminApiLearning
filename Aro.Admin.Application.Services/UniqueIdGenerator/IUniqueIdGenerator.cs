using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.UniqueIdGenerator;

public interface IUniqueIdGenerator : IService
{
    Guid Generate();
}
