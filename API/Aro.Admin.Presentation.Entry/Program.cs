using Aro.Admin.Presentation.Entry.Extensions;
using AspNetCoreRateLimit;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.InstallServices();

    // Configure the HTTP request pipeline.

    var app = builder.Build();

    app.UseGlobalExceptionHandler();
    app.UseRequestLogging();

    // CORS must be before rate limiting so CORS headers are added to all responses
    app.UseRouting();
    app.UseCors(app.Environment.EnvironmentName);

    // Rate limiting middleware - must be before authentication but after CORS
    app.UseIpRateLimiting();

    await app.MigrateDatabase(builder.Configuration).ConfigureAwait(false);
    await app.SeedDatabase(Path.Combine(@"AppData\PermissionSeed.json"), @"AppData\EmailTemplates").ConfigureAwait(false);
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