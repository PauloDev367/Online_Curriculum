using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Options;
using OnlineCurriculum.Configurations;

namespace OnlineCurriculum.Extensions;

public static class ConfigureS3Extension
{
    public async static Task ConfigureS3(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3Settings"));
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.RegionName),
            };
            var accessKey = s3Settings.AccessKey;
            var secretKey = s3Settings.SecretKey;
            return new AmazonS3Client(accessKey, secretKey, config);
        });
    }
}