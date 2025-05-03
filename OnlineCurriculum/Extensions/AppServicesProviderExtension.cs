using OnlineCurriculum.Services;

namespace OnlineCurriculum.Extensions;

public static class AppServicesProviderExtension
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddTransient<IdentityService>();
        services.AddTransient<CandidateProfileService>();
        services.AddTransient<RecruiterService>();
        services.AddTransient<S3Service>();
        services.AddTransient<ResumeFileService>();
    }
}