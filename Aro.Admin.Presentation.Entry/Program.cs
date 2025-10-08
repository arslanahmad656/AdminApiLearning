using Aro.Admin.Presentation.Entry.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.InstallServices();

var app = builder.Build();

await app.MigrateDatabase().ConfigureAwait(false);
await app.SeedDatabase(Path.Combine(@"AppData\PemissionSeed.json")).ConfigureAwait(false);

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
