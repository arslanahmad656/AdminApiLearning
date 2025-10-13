namespace Aro.Admin.Application.Services;

public interface ISystemContext : IService
{
    public bool IsSystemContext { get; set; }
}
