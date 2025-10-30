
using Aro.Admin.Application.Shared.Options;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class OptionsInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<LoggingSettings>(builder.Configuration.GetSection("LoggingSettings"));
        builder.Services.Configure<PasswordResetSettings>(builder.Configuration.GetSection("PasswordResetSettings"));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.Configure<BootstrapUserSettings>(builder.Configuration.GetSection("BootstrapUser"));
        builder.Services.Configure<PasswordHistorySettings>(builder.Configuration.GetSection("PasswordHistorySettings"));
        builder.Services.Configure<PasswordPolicyOptions>(builder.Configuration.GetSection("PasswordPolicy"));
    }
}
