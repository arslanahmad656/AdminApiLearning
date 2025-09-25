namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class AutoMapperInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
    }
}
