using Microsoft.EntityFrameworkCore;
using OnlineCurriculum.Data;

namespace OnlineCurriculum.Extensions;

public static class LambdaServiceProviderExtension
{
    public static void ConfigureLambdaDependencies(this IServiceCollection services)
    {
        DotNetEnv.Env.Load();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SqlServer")
                               ?? throw new InvalidOperationException("Missing ConnectionStrings__SqlServer environment variable");
        Console.WriteLine("🔗 ConnectionString from ENV: " + connectionString);
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddMemoryCache();
        services.AddHttpClient();

        services.ConfigureDependencies();
    }
}