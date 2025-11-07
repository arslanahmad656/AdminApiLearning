namespace Aro.Common.Application.Services.SystemContext;

public interface ISystemContextFactory
{
    ISystemContextEnabler Create();
}