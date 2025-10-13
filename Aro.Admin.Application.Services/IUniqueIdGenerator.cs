namespace Aro.Admin.Application.Services;

public interface IUniqueIdGenerator : IService
{
    Guid Generate();
}
