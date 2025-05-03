using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineCurriculum.Configurations;
using OnlineCurriculum.Data;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class S3Service
{
    private readonly S3Settings _settings;
    private readonly IAmazonS3 _client;
    private readonly IMemoryCache _cache;
    public S3Service(
        IAmazonS3 s3Client,
        IOptions<S3Settings> S3Settings, IMemoryCache cache)
    {
        _client = s3Client;
        _cache = cache;
        _settings = S3Settings.Value;
    }

    public async Task<CreatedBucketResponse> UploadFileAsync(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("No file uploaded");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!extension.Equals(".pdf") && !extension.Equals(".docx"))
        {
            throw new Exception("Only .pdf and .docx files are supported");
        }

        const long maxFileSize = 5 * 1024 * 1024;

        if (file.Length > maxFileSize)
        {
            throw new Exception("File too large! The maximum is 5MB");
        }

        using var stream = file.OpenReadStream();
        var key = Guid.NewGuid();

        var putRequest = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = $"curriculums/{key}",
            InputStream = stream,
            ContentType = file.ContentType,
            Metadata =
            {
                ["original-file-name"] = file.FileName,
                ["original-file-extension"] = file.FileName,
            }
        };
        await _client.PutObjectAsync(putRequest);
        var response = new CreatedBucketResponse
        {
            FileName = key.ToString(),
            FileSize = file.Length,
            OriginalName = file.FileName,
            ContentType = file.ContentType,
        };
        return response;
    }

    public async Task RemoveFileAsync(string key)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = $"curriculums/{key}",
        };
        await _client.DeleteObjectAsync(deleteRequest);
    }

    public async Task<string> GetFileFromBucketAsync(string key)
    {
        var cacheKey = "curriculums-" + key;

        if (!_cache.TryGetValue(cacheKey, out string presignedUrl))
        {
            var getRequest = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = $"curriculums/{key}",
                Verb = HttpVerb.GET,
                Expires = DateTime.Now.AddHours(1)
            };
        
            presignedUrl = await _client.GetPreSignedURLAsync(getRequest);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _cache.Set(cacheKey, presignedUrl, cacheEntryOptions);
        }
        
        return presignedUrl;
    }
}