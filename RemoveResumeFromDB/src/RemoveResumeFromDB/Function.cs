using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using OnlineCurriculum.Services;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace RemoveResumeFromDB;

public class Function
{
    private static ResumeFileService? _service;

    static Function()
    {
        var startup = new Startup();
        var services = new ServiceCollection();
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        _service = provider.GetRequiredService<ResumeFileService>();
    }

    [LambdaFunction]
    public async Task FunctionHandlerAsync(string fileName, ILambdaContext context)
    {
        await _service!.RemoveFile(fileName);
    }
}