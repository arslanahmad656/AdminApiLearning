namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class CorsInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        var corsPolicies = builder.Configuration
            .GetSection("Cors:Policies")
            .Get<List<CorsPolicyConfig>>() ?? [];

        builder.Services.AddCors(options =>
        {
            foreach (var policy in corsPolicies)
            {
                options.AddPolicy(policy.Name, builder =>
                {
                    if (policy.AllowedOrigins.Contains("*"))
                        builder.AllowAnyOrigin();
                    else
                        builder.WithOrigins([.. policy.AllowedOrigins]);

                    if (policy.AllowedMethods.Contains("*"))
                        builder.AllowAnyMethod();
                    else
                        builder.WithMethods([.. policy.AllowedMethods]);

                    if (policy.AllowedHeaders.Contains("*"))
                        builder.AllowAnyHeader();
                    else
                        builder.WithHeaders([.. policy.AllowedHeaders]);

                    if (policy.ExposedHeaders.Count != 0)
                        builder.WithExposedHeaders([.. policy.ExposedHeaders]);
                });
            }
        });
    }
}

file class CorsPolicyConfig
{
    public string Name { get; set; } = default!;
    public List<string> AllowedOrigins { get; set; } = [];
    public List<string> AllowedMethods { get; set; } = [];
    public List<string> AllowedHeaders { get; set; } = [];
    public List<string> ExposedHeaders { get; set; } = [];
}
