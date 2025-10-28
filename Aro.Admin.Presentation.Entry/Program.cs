using Aro.Admin.Presentation.Entry.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.InstallServices();

    var app = builder.Build();

    app.UseGlobalExceptionHandler();
    app.UseRequestLogging();

    await app.MigrateDatabase().ConfigureAwait(false);
    await app.SeedDatabase(Path.Combine(@"AppData\PemissionSeed.json"), @"AppData\EmailTemplates").ConfigureAwait(false);
    await app.CreateBootstrapUser().ConfigureAwait(false);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}