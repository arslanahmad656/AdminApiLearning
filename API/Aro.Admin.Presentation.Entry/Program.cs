using Aro.Admin.Presentation.Entry.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.InstallServices();

    // Configure the HTTP request pipeline.

    var app = builder.Build();

    app.UseGlobalExceptionHandler();
    app.UseRequestLogging();

    await app.MigrateDatabase().ConfigureAwait(false);
    await app.SeedDatabase(Path.Combine(@"AppData\PermissionSeed.json"), @"AppData\EmailTemplates").ConfigureAwait(false);
    await app.CreateBootstrapUser().ConfigureAwait(false);

    var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwaggerUI");
    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseCors(app.Environment.EnvironmentName);

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