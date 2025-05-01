using OnlineCurriculum.Services;

namespace OnlineCurriculum.Extensions;

public static class AppServicesProviderExtension
{
    public static void ConfigureIdentityAuth(this IServiceCollection services)
    {
        services.AddTransient<IdentityService>();
        services.AddTransient<CandidateProfileService>();
        services.AddTransient<RecruiterService>();
    }
}