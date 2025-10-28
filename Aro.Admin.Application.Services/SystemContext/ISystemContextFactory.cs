namespace Aro.Admin.Application.Services.SystemContext;

public interface ISystemContextFactory
{
    ISystemContextEnabler Create();
}