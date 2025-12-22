using Aro.Admin.Presentation.Entry.Extensions;
using AspNetCoreRateLimit;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.InstallServices();

    var app = builder.Build();

    app.UseGlobalExceptionHandler();
    app.UseRequestLogging();

    app.UseRouting();
    app.UseCors(app.Environment.EnvironmentName);

    app.UseIpRateLimiting();

    await app.MigrateDatabase(builder.Configuration).ConfigureAwait(false);
    await app.SeedDatabase(Path.Combine(@"AppData\PermissionSeed.json"), Path.Combine(@"AppData\EmailTemplates"), Path.Combine(@"AppData\CountryMetadata.json")).ConfigureAwait(false);
    await app.CreateBootstrapUser().ConfigureAwait(false);

    var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwaggerUI");
    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.EnableConfigurationSettings();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}