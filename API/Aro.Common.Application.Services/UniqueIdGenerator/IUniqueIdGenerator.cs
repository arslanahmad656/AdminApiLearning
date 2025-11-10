namespace Aro.Common.Application.Services.UniqueIdGenerator;

public interface IUniqueIdGenerator : IService
{
    Guid Generate();
}
