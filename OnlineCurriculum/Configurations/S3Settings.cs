namespace OnlineCurriculum.Configurations;

public class S3Settings
{
    public string RegionName { get; init; } = "us-east-2";
    public string BucketName { get; init; } = "bucketesetudocsharps3";
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
}