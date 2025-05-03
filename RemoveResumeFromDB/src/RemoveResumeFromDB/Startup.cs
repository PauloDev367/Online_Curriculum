using Amazon.Lambda.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OnlineCurriculum.Extensions;

namespace RemoveResumeFromDB;
[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureLambdaDependencies();
    }
}